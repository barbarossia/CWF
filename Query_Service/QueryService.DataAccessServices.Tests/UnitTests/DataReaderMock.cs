using System.Data;
using System.Collections;

public class DataReaderMock : IDataReader
{
    private DataTable table = null;
    private IEnumerator enumerator;
    public DataReaderMock(DataTable data)
    {
        this.table = data;
        enumerator = data.Rows.GetEnumerator();
    }
    public void Close()
    {
        Dispose();
    }

    public int Depth
    {
        get { throw new System.NotImplementedException(); }
    }

    public DataTable GetSchemaTable()
    {
        throw new System.NotImplementedException();
    }

    public bool IsClosed
    {
        get { throw new System.NotImplementedException(); }
    }

    public bool NextResult()
    {
        throw new System.NotImplementedException();
    }

    public bool Read()
    {
        return this.enumerator.MoveNext();
    }

    public int RecordsAffected
    {
        get { throw new System.NotImplementedException(); }
    }

    public void Dispose()
    {
        
    }

    public int FieldCount
    {
        get
        {
            return ((DataRow)this.enumerator.Current).ItemArray.Length;
        }
    }

    public bool GetBoolean(int i)
    {
        throw new System.NotImplementedException();
    }

    public byte GetByte(int i)
    {
        throw new System.NotImplementedException();
    }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
        throw new System.NotImplementedException();
    }

    public char GetChar(int i)
    {
        throw new System.NotImplementedException();
    }

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
        throw new System.NotImplementedException();
    }

    public IDataReader GetData(int i)
    {
        throw new System.NotImplementedException();
    }

    public string GetDataTypeName(int i)
    {
        throw new System.NotImplementedException();
    }

    public System.DateTime GetDateTime(int i)
    {
        throw new System.NotImplementedException();
    }

    public decimal GetDecimal(int i)
    {
        throw new System.NotImplementedException();
    }

    public double GetDouble(int i)
    {
        throw new System.NotImplementedException();
    }

    public System.Type GetFieldType(int i)
    {
        throw new System.NotImplementedException();
    }

    public float GetFloat(int i)
    {
        throw new System.NotImplementedException();
    }

    public System.Guid GetGuid(int i)
    {
        throw new System.NotImplementedException();
    }

    public short GetInt16(int i)
    {
        throw new System.NotImplementedException();
    }

    public int GetInt32(int i)
    {
        throw new System.NotImplementedException();
    }

    public long GetInt64(int i)
    {
        throw new System.NotImplementedException();
    }

    public string GetName(int i)
    {
        throw new System.NotImplementedException();
    }

    public int GetOrdinal(string name)
    {
        throw new System.NotImplementedException();
    }

    public string GetString(int i)
    {
        throw new System.NotImplementedException();
    }

    public object GetValue(int i)
    {
        throw new System.NotImplementedException();
    }

    public int GetValues(object[] values)
    {
        throw new System.NotImplementedException();
    }

    public bool IsDBNull(int i)
    {
        throw new System.NotImplementedException();
    }

    public object this[string name]
    {
        get
        {
            return ((DataRow)this.enumerator.Current)[name];
        }
    }

    public object this[int i]
    {
        get
        {
            return ((DataRow)this.enumerator.Current)[i];
        }
    }
}