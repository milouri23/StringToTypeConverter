using StringToTypeConverter.Model;

Console.WriteLine("Hello, World!");

var variables = new List<Variable>
{
    new("intVar", TipoDato.Int, "Variable de tipo int"),
    new("stringVar", TipoDato.String, "Variable de tipo string"),
    new("boolVar", TipoDato.Bool, "Variable de tipo bool"),
    new("intArray", TipoDato.Int, "Arreglo de tipo int", true),
    new("stringArray", TipoDato.String, "Arreglo de tipo string", true),
    new("boolArray", TipoDato.Bool, "Arreglo de tipo bool", true)
};

var valores = new List<string> { "123", "Hello, World!", "true", "1,2,3", "hola,mundo,feliz", "true,false,true,false" };

for (int i = 0; i < variables.Count; i++)
{
    try
    {
        var valorConvertido = variables[i].ConvertirValor(valores[i]);
        var message = valorConvertido switch
        {
            Array array => $"El valor convertido de {variables[i].Nombre} es: {string.Join(", ", array.Cast<object>())}",
            _ => $"El valor convertido de {variables[i].Nombre} es: {valorConvertido}"
        };

        Console.WriteLine(message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error converting value: {ex.Message}");
    }
}

namespace StringToTypeConverter.Model
{
    public record Variable(string Nombre, TipoDato Tipo, string Descripcion = "", bool EsArray = false)
    {
        public object ConvertirValor(string stringValue)
        {
            try
            {
                return EsArray ? ParseArray(stringValue, Tipo) : ParseValue(stringValue, Tipo);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not convert the value '{stringValue}' to type {Tipo}", ex);
            }
        }

        private static object ParseValue(string stringValue, TipoDato tipo)
            => tipo switch
            {
                TipoDato.Int when int.TryParse(stringValue, out var intValue) => intValue,
                TipoDato.String => stringValue,
                TipoDato.Bool when bool.TryParse(stringValue, out var boolValue) => boolValue,
                _ => throw new ArgumentException($"TipoVariable {tipo} not supported")
            };

        private static object ParseArray(string stringValue, TipoDato tipo)
        {
            if (stringValue is null)
            {
                throw new ArgumentNullException(nameof(stringValue));
            }

            var values = stringValue.Split(',');
            return tipo switch
            {
                TipoDato.Int => values.Select(value => int.TryParse(value, out var result) ? result : throw new ArgumentException($"Failed to parse '{value}' as int")).ToArray(),
                TipoDato.String => values,
                TipoDato.Bool => values.Select(value => bool.TryParse(value, out var result) ? result : throw new ArgumentException($"Failed to parse '{value}' as bool")).ToArray(),
                _ => throw new ArgumentException($"TipoVariable {tipo} not supported")
            };
        }
    }

    public enum TipoDato
    {
        Int,
        String,
        Bool
    }
}