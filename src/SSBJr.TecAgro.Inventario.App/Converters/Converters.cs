using SSBJr.TecAgro.Inventario.Domain.Entities;
using System.Globalization;

namespace SSBJr.TecAgro.Inventario.App.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
  if (value is StatusSincronizacao status)
        {
        return status switch
         {
                StatusSincronizacao.Sincronizado => Colors.Green,
      StatusSincronizacao.Pendente => Colors.Orange,
  StatusSincronizacao.Erro => Colors.Red,
         StatusSincronizacao.EmProcessamento => Colors.Blue,
 _ => Colors.Gray
        };
        }
return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
  throw new NotImplementedException();
    }
}

public class StatusToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
   if (value is StatusSincronizacao status)
        {
      return status switch
  {
             StatusSincronizacao.Sincronizado => "Sincronizado",
          StatusSincronizacao.Pendente => "Pendente",
StatusSincronizacao.Erro => "Erro",
     StatusSincronizacao.EmProcessamento => "Processando",
 _ => "Desconhecido"
       };
        }
 return "Desconhecido";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
}

public class BoolToOnlineConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
    if (value is bool isOnline)
      {
     return isOnline ? "Online" : "Offline";
     }
     return "Offline";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
throw new NotImplementedException();
    }
}
