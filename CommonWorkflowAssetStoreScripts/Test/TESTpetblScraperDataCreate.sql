USE [Argus]

/*
 * Run and ensure that the en-US and ja-JP entries all have XML data.
 * The ko-kr entries should be null
 */
SELECT l.LanguageName, p.ProductName, [fileName], StatusCode, [Data]
FROM etblUiStorageXml t
JOIN ltblLanguages l ON l.LanguageID = t.LanguageID
JOIN ltblProducts p ON p.ProductID = t.ProductID