using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace inmobiliaria.Models;

public class Expresiones 
{ 
    private static string  eNombre = @"^[A-Za-z]+$";
    private static string eApellido = @"^[A-Za-z']+$";
    private static string eTelefono = @"^[0-9]+$";
    private static string eMail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    private static string eDNI= @"^[0-9]{7,8}$";
    private static string eClave = @"^(?=.*[A-Z])(?=.*[@#$%^&+=!]).{8,}$";
    private static string eLatitud = @"^-?([0-9]|[1-8][0-9]|90)(\.[0-9]{1,6})?$";
    private static string eLongitud = @"^-?([0-9]|[1-9][0-9]|1[0-7][0-9]|180)(\.[0-9]{1,6})?$";
    public static bool ValidarNombre(string n){
        return Regex.IsMatch(n, eNombre);
    }
    public static bool ValidarApellido(string a){
        return Regex.IsMatch(a, eApellido);
    }
    public static bool ValidarTelefono(string t){
        return Regex.IsMatch(t, eTelefono);
    }
     public static bool ValidarMail(string m){
        return Regex.IsMatch(m, eMail);
    }
     public static bool ValidarDNI(string d){
        return Regex.IsMatch(d, eDNI);
    }
     public static bool ValidarClave(string c){
        return Regex.IsMatch(c, eClave);
    }
    public static bool ValidarLongitud(string l){
        return Regex.IsMatch(l, eLongitud);
    }
     public static bool ValidarLatitud(string l){
        return Regex.IsMatch(l, eLatitud);
    }
}