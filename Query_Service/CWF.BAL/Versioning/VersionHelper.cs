using System;
using System.Collections.Generic;
using System.Linq;
using CWF.DAL;
using CWF.DataContracts;
using CWF.DataContracts.Versioning;
using Microsoft.Support.Workflow.Service.BusinessServices;
using System.Text.RegularExpressions;
using Microsoft.Support.Workflow.Service.DataAccessServices;

namespace CWF.BAL.Versioning
{
    /// <summary>
    /// This class implements the business rules for versioning.
    /// </summary>
    public static class VersionHelper
    {
        private const int MaximumMinorSectionValue = 9999;

        private const string MustBeIncrementedMessage = "\r\nThe {0} must be incremented.";
        private const string MustBeResetMessage = "\r\nThe {0} must be reset to {1}.";
        private const string NameMustBeChangedMessage = "\r\nThe name must be changed.";
        private const string NoRuleDefinedMessage = "There is no rule defined for the state {0}, {1}";
        private const string InvalidDependencyStateMessage = "One or more dependencies have a less restrictive state than {0}, which is the most restrictive state in the list of Activities being uploaded.\r\n";
        private const string HasRetiredStateMessage = "One or more dependencies are retired.\r\n";
        private const string DefaultDependencyState = "Public";
        private const string InvalidVersionNumberMessage = "The version number specified, '{0}', is not a valid Marketplace version number.";

        // These constants exist to make the following array of rules more readable
        private const RequiredChange Reset = RequiredChange.MustReset;
        private const RequiredChange Change = RequiredChange.MustChange;
        private const RequiredChange Increment = RequiredChange.MustIncrement;
        private const RequiredChange NoAction = RequiredChange.NoActionRequired;
        private const RequestedOperation AddNew = RequestedOperation.AddNew;
        private const RequestedOperation Update = RequestedOperation.Update;
        private const RequestedOperation Compile = RequestedOperation.Compile;

        private static readonly Rule[] versioningRules = new[] 
        {
            //                                  Name -    Major -   Minor -   Build -    Revision 
            //       Action                     Required  Required  Required  Required   Required 
            //       Type     Public? Retired?  Action    Action    Action    Action,    Action
            //=============================================================================================
            new Rule(AddNew,  false,  false,    Change,   Reset,     Reset,     Reset,     Reset),     // Requires the user to save as a new workflow with version 1.0 .
            new Rule(AddNew,  false,  true,     Change,   Reset,     Reset,     Reset,     Reset),     // The workflow is in a retired state. It must be renamed and saved as a new workflow, with version 1.0 .
            new Rule(AddNew,  true,   false,    Change,   NoAction,  Increment, NoAction,  NoAction),  // Since the workflow is in a public state, it must be moved to a new workflow, with version 1.0, before it can be edited.

            new Rule(Update,  false,  false,    NoAction, NoAction,  NoAction,  NoAction,  Increment), // The user is attempting to update another user's private workflow. The workflow must be saved as a new workflow with verion 1.0 before they can continue.
            new Rule(Update,  false,  true,     Change,   Reset,     Reset,     Reset,     Reset),     // Since the workflow is retired, it must be moved to a new workflow, with version 1.0, before it can be edited.
            new Rule(Update,  true,   false,    NoAction, NoAction,  Increment, NoAction,  NoAction),     // Since the workflow is in a public state, it must be moved to a new workflow, with version 1.0, before it can be edited.
 
            new Rule(Compile, false,  false,    NoAction, NoAction,  NoAction,  Increment, NoAction),  // The compile functionality controls the Build field. The user can compile another user's work, and the version does change, but Major, Minor and Revision do not.
            new Rule(Compile, false,  true,     NoAction, NoAction,  NoAction,  Increment, NoAction),  // The compile functionality controls the Build field. The user can compile another user's work, and the version does change, but Major, Minor and Revision do not.
            new Rule(Compile, true,   false,    NoAction, NoAction,  NoAction,  Increment, NoAction),  // The compile functionality controls the Build field. The user can compile another user's work, and the version does change, but Major, Minor and Revision do not. 
        };


        private static readonly Dictionary<Section, int> versionSectionResetValues = new Dictionary<Section, int>
        {
           {Section.Major,    0},
           {Section.Minor,    0},
           {Section.Build,    0},
           {Section.Revision, 1},
        };

        /// <summary>
        ///  Checks the rules for versioning. If the rules are incorrect, it returns false and a string detailing what the error 
        ///  was, and a human readable string to display to the user.
        /// </summary>
        /// <param name="workflowToCheck">the StoreActivity record we care about versioning</param>
        /// <param name="requestedOperation">The state of the object being saved. I.e., is this a new record, an update, etc.
        /// Save, AddNew, Compile (and for future use), Delete. "Null" means "figure it out from what's in the database"</param>
        /// <param name="workflowRecordState">Public, Private, Retired. </param>
        /// <param name="env">the user making the request</param>
        /// <returns>
        /// a tuple with a bool indicating pass/fail and a description of what needs to change (if any)
        /// Message is a message describing the problem, and Rule is the rule that was applied/tested against.
        /// </returns>
        public static Tuple<bool, string, Rule> CheckVersioningRules(StoreActivitiesDC workflowToCheck,
                                                                     RequestedOperation? requestedOperation,
                                                                     string env)
        {
            var isRulePassed = true;
            bool isPublic;
            bool isRetired;
            var errorString = String.Empty;
            var previousVersions = Activities.StoreActivitiesGetByName(workflowToCheck.Name, env);
            Rule rule = null;

            if (!IsValidMarketplaceVersion(workflowToCheck.Version))
                throw new VersionException(string.Format(InvalidVersionNumberMessage, workflowToCheck.Version), null);

            previousVersions = previousVersions.Where(item => Version.Parse(item.Version) >= Version.Parse(workflowToCheck.Version)).ToList();
            if (null == requestedOperation)
                requestedOperation = previousVersions.Any()
                    ? RequestedOperation.Update : RequestedOperation.AddNew;

            if (!string.IsNullOrEmpty(workflowToCheck.StatusCodeName))
                workflowToCheck.WorkflowRecordState = (WorkflowRecordState)Enum.Parse(typeof(WorkflowRecordState), workflowToCheck.StatusCodeName);

            if (null == workflowToCheck.WorkflowRecordState)
                workflowToCheck.WorkflowRecordState = WorkflowRecordState.Private;

            isPublic = workflowToCheck.WorkflowRecordState == WorkflowRecordState.Public;
            isRetired = workflowToCheck.WorkflowRecordState == WorkflowRecordState.Retired;

            if ((previousVersions.Count == 0) && !isPublic)
                return new Tuple<bool, string, Rule>(true, String.Empty, null);

            // find the rule that matches the current condition
            var ruleQuery = (from candidateRule in versioningRules
                             where (candidateRule.ActionType == requestedOperation)
                                && (candidateRule.IsPublic == isPublic)
                                && (candidateRule.IsRetired == isRetired)
                             select candidateRule)
                            .ToList();

            if (ruleQuery.Any())
            {
                rule = ruleQuery.First();

                if (rule.NameRequiredChange != RequiredChange.NoActionRequired)
                {
                    // VersionRequiredActionEnum.MustChange  is the only valid action here - check to make sure this name does not exist in the database
                    isRulePassed = (previousVersions.Count == 0);
                    if (!isRulePassed)
                        errorString += string.Format(NameMustBeChangedMessage);
                }

                var major = GetVersionSection(workflowToCheck.Version, Section.Major);

                if ((from workflow in previousVersions
                     where GetVersionSection(workflow.Version, Section.Major) > major
                     select workflow).Any())
                {
                    isRulePassed = false;
                    errorString += string.Format(MustBeIncrementedMessage, Section.Major.ToString());
                }

                // when checking minor, build, revision, we only care about number ranges within the major build.
                // for instance, there can be a build 5 in major version 2, even though the max build number might be 10 in another major version

                previousVersions = (from workflow in previousVersions
                                    where GetVersionSection(workflow.Version, Section.Major) == major
                                    select workflow)
                                   .ToList();

                isRulePassed &= CheckVersionSection(rule, Section.Major, previousVersions, workflowToCheck, ref errorString);
                isRulePassed &= CheckVersionSection(rule, Section.Minor, previousVersions, workflowToCheck, ref errorString);
                isRulePassed &= CheckVersionSection(rule, Section.Build, previousVersions, workflowToCheck, ref errorString);
                isRulePassed &= CheckVersionSection(rule, Section.Revision, previousVersions, workflowToCheck, ref errorString);
            }
            else
            {
                // TODO: Provide the correct associated error code when the error handling refactoring is performed.
                throw new BusinessException(-1, string.Format(NoRuleDefinedMessage, requestedOperation, workflowToCheck.WorkflowRecordState));
            }

            return new Tuple<bool, string, Rule>(isRulePassed, errorString.Trim(), rule);
        }

        private static int GetVersionSection(string versionString, Section section)
        {
            var theVersion = Version.Parse(versionString);

            // make a lookup so we can easily determine the section without a switch statement
            var sectionValues = new Dictionary<Section, int>
                                  {
                                     {Section.Major,    theVersion.Major},
                                     {Section.Minor,    theVersion.Minor},
                                     {Section.Build,    theVersion.Build},
                                     {Section.Revision, theVersion.Revision},
                                  };

            return sectionValues.First(ruleAction1 => ruleAction1.Key == section).Value;
        }

        private static bool CheckVersionSection(Rule rule,
                                                Section section,
                                                IEnumerable<StoreActivitiesDC> previousVersions,
                                                StoreActivitiesDC workflowToTest,
                                                ref string errorString)
        {
            var isRulePassed = true; // defaults to a state of passing the section

            // make a lookup so we can easily determine the required change, without resorting to a switch statement or similar
            var ruleActions = new Dictionary<Section, RequiredChange>
                                  {
                                     {Section.Major,    rule.MajorRequiredChange},
                                     {Section.Minor,    rule.MinorRequiredChange},
                                     {Section.Build,    rule.BuildRequiredChange},
                                     {Section.Revision, rule.RevisionRequiredChange},
                                  };

            var ruleAction = ruleActions.First(ruleAction1 => ruleAction1.Key == section).Value;

            if (ruleAction != RequiredChange.NoActionRequired)
            {
                var sectionValue = GetVersionSection(workflowToTest.Version, section); // the value of the specified piece of the version number 

                switch (ruleAction)
                {
                    case RequiredChange.MustIncrement:
                        if (previousVersions.Any())
                            isRulePassed = previousVersions
                                               .Select(workflow => GetVersionSection(workflow.Version, section))
                                               .Max() < sectionValue;
                        if (!isRulePassed)
                            errorString += string.Format(MustBeIncrementedMessage, section.ToString());

                        break;

                    case RequiredChange.MustReset:
                        isRulePassed = (sectionValue == versionSectionResetValues[section]);
                        if (!isRulePassed)
                            errorString += string.Format(MustBeResetMessage, section, versionSectionResetValues[section]);

                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            return isRulePassed;
        }

        public static Version GetNextVersion(StoreActivitiesDC request)
        {
            Func<int, int> incrementOnlyAction = (i => i + 1); // the lambda to pass in to the section handling routine when all we need is a simple increment
            var workflowToTest = request;
            var existingRecords = Activities.StoreActivitiesGetByName(workflowToTest.Name, request.Environment);
            int major;
            int minor;
            int build;
            int revision;
            Version theVersion;
            Rule rule;

            if (!IsValidMarketplaceVersion(request.Version))
                throw new VersionException(string.Format(InvalidVersionNumberMessage, request.Version), null);

            Version maxVersion = (from record in existingRecords
                                  select new Version(record.Version))
                                 .Max();


            if (null == maxVersion)
                return new Version(
                                     versionSectionResetValues[Section.Major],
                                     versionSectionResetValues[Section.Minor],
                                     versionSectionResetValues[Section.Build],
                                     versionSectionResetValues[Section.Revision]
                                  );


            if (new Version(request.Version) < maxVersion)
                request.Version = maxVersion.ToString();

            theVersion = new Version(request.Version);
            major = theVersion.Major;
            minor = theVersion.Minor;
            build = theVersion.Build;
            revision = theVersion.Revision;

            rule = CheckVersioningRules(workflowToTest, null, request.Environment).Item3;

            major = HandleVersionSectionChange(rule.MajorRequiredChange,
                                              initialValue =>
                                              {
                                                  minor = versionSectionResetValues[Section.Minor];
                                                  build = versionSectionResetValues[Section.Build];
                                                  revision = versionSectionResetValues[Section.Revision];
                                                  return initialValue + 1;
                                              },
                                              major,
                                              versionSectionResetValues[Section.Major]);


            minor = HandleVersionSectionChange(rule.MinorRequiredChange,
                                               initialValue =>
                                               {
                                                   var result = initialValue + 1;
                                                   if (result > MaximumMinorSectionValue)
                                                   {
                                                       major++;
                                                       result = versionSectionResetValues[Section.Minor];
                                                   }
                                                   return result;
                                               },
                                               minor,
                                               versionSectionResetValues[Section.Minor]);

            build = HandleVersionSectionChange(rule.BuildRequiredChange, incrementOnlyAction, build, versionSectionResetValues[Section.Build]);
            revision = HandleVersionSectionChange(rule.RevisionRequiredChange, incrementOnlyAction, revision, versionSectionResetValues[Section.Revision]);

            return new Version(major, minor, build, revision);
        }

        private static int HandleVersionSectionChange(RequiredChange change, Func<int, int> incrementAction, int initialSectionValue, int resetValue)
        {
            int result = initialSectionValue;

            switch (change)
            {
                case RequiredChange.MustIncrement:
                    result = incrementAction(result);
                    break;

                case RequiredChange.MustReset:
                    result = resetValue;
                    break;
            }

            return result;
        }

        public static WorkflowRecordState GetWorkflowRecordState(string publishingState)
        {
            WorkflowRecordState result;
            var success = Enum.TryParse<WorkflowRecordState>(publishingState, true, out result);

            if (!success)
                result = WorkflowRecordState.Public;

            return result;
        }


        /// <summary>
        /// Examines a dependency tree to determine if the state of the dependencies is compatible
        /// with the state of the StoreActivity records being persisted/checked.
        /// 
        /// -- "Retired" in any dependency means that the entire tree is invalid.
        /// -- "Private" in any dependency when going public means the tree is invalid.
        /// 
        /// </summary>
        /// <param name="request">The tree to examine.</param>
        /// <returns>A tuple containing true if everything passed, or false and a message if the tests failed.</returns>
        public static Tuple<bool, string> CheckDependencyRules(StoreLibraryAndActivitiesRequestDC request)
        {
            Action<List<StoreActivityLibraryDependenciesGroupsRequestDC>> findDependencies = null;
            var allDependencies = new Dictionary<Tuple<string, string>, StoreActivityLibraryDependenciesGroupsRequestDC>();
            bool hasInvalidDependencyState;
            bool hasRetiredState;
            string message = String.Empty;

            findDependencies = list =>
              {
                  if (null != list)
                      list.ForEach(item =>
                      {
                          var key = new Tuple<string, string>(item.Name, item.Version);

                          if (!allDependencies.ContainsKey(key))
                              allDependencies.Add(key, item);
                          findDependencies(item.List);
                      });
              };

            findDependencies(request.StoreActivityLibraryDependenciesGroupsRequestDC.List);
            GetMissingDependencyStates(allDependencies.Values);

            if (request.StoreActivitiesList.Any())
            {
                var rootVersions = request
                                 .StoreActivitiesList
                                 .Select(item => GetWorkflowRecordState(item.StatusCodeName));
                var mostRestrictiveRootState = rootVersions.Min();

                hasInvalidDependencyState = allDependencies.Values.Any(entry => GetWorkflowRecordState(entry.Status) < mostRestrictiveRootState);
                if (hasInvalidDependencyState)
                    message += string.Format(InvalidDependencyStateMessage, mostRestrictiveRootState);

                hasRetiredState = allDependencies.Values.Any(entry => GetWorkflowRecordState(entry.Status) == WorkflowRecordState.Retired);
                if (hasRetiredState)
                    message += HasRetiredStateMessage;

                return new Tuple<bool, string>(!(hasInvalidDependencyState || hasRetiredState), message.Trim());
            }

            return new Tuple<bool, string>(true, string.Empty);
        }


        /// <summary>
        /// For any dependencies that have no value for their Status field, determine from the database what that state should be. 
        /// </summary>
        /// <param name="allDependencies"></param>
        public static void GetMissingDependencyStates(IEnumerable<StoreActivityLibraryDependenciesGroupsRequestDC> dependencies)
        {
            dependencies
                .Where(dependency => string.IsNullOrEmpty(dependency.Status))
                .ToList()
                .ForEach(dependency =>
                {
                    var activity = ActivityLibrary.ActivityLibraryGet(
                                                                    new ActivityLibraryDC { Name = dependency.Name, VersionNumber = dependency.Version, },
                                                                    false
                                                                )
                                                                .FirstOrDefault();

                    if (null != activity)
                        dependency.Status = activity.StatusName;
                    else
                        dependency.Status = DefaultDependencyState;
                });

        }

        /// <summary>
        ///  Determines if the version (passed as a string) meets the requirements to be a Marketplace version number.
        /// 
        /// -- The Version object in the framework allows versions with more or less than 4 positions -- such as, 1.2, 0.0.1, 
        ///    and 1.2.3.4.5.6 are all parseable version numbers, using Version.Parse().  We require 4 positions.
        ///    
        /// -- Each of the positions must be 1 or two digits. That is, they are in the range of 0-99.
        /// 
        /// </summary>
        /// <param name="version"></param>
        public static bool IsValidMarketplaceVersion(string version)
        {
            Regex pattern = new Regex(@"^[0-9]{1,2}\.[0-9]{1,4}\.[0-9]{1,2}\.[0-9]{1,2}$");

            return pattern.IsMatch(version);
        }

    }
}