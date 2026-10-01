using System;
using System.Collections.Generic;
using System.Data;

public class EntidadeFuncoes<T> where T : new()
{
    public List<T> ConverterDataSet(DataSet dataSet, string tableName)
    {
        List<T> objetos = new List<T>();

        foreach (DataRow row in dataSet.Tables[tableName].Rows)
        {
            T objeto = new T();

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (dataSet.Tables[tableName].Columns.Contains(propertyInfo.Name))
                {
                    var value = row[propertyInfo.Name];
                    if (value != DBNull.Value)
                    {
                        propertyInfo.SetValue(objeto, value);
                    }
                }
            }

            objetos.Add(objeto);
        }

        return objetos;
    }

    public static List<T> ConvertDataTable(DataTable dataTable)
    {
        List<T> objetos = new List<T>();
        if (dataTable == null || dataTable.Rows.Count == 0) return objetos;

        var properties = typeof(T).GetProperties();

        foreach (DataRow row in dataTable.Rows)
        {
            T objeto = new T();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanWrite && dataTable.Columns.Contains(propertyInfo.Name))
                {
                    var value = row[propertyInfo.Name];
                    if (value != DBNull.Value)
                    {
                        try
                        {
                            // Lida com tipos anuláveis (ex: int?, DateTime?)
                            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                            // Converte o valor para o tipo da propriedade de forma segura
                            var safeValue = Convert.ChangeType(value, propertyType);

                            propertyInfo.SetValue(objeto, safeValue, null);
                        }
                        catch (Exception)
                        {
                            // Opcional: Logar erro se a conversão falhar.
                            // Por padrão, simplesmente ignora a propriedade que não pôde ser convertida.
                        }
                    }
                }
            }
            objetos.Add(objeto);
        }
        return objetos;
    }
}