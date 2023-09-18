namespace inmobiliaria.Models;
using System.Data;
using MySql.Data.MySqlClient;
public class UsuariosRepositorio
{
    public List<Usuarios> ObtenerTodos()
        {
            var res = new List<Usuarios>();
            try{
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
                {
                    string sql = @"SELECT Id,DNI,Nombre,Apellido,Telefono,Mail FROM Usuarios;";
                    using (MySqlCommand command= new MySqlCommand(sql,connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        var reader = command.ExecuteReader();
                        while(reader.Read())
                        {
                            Usuarios r = new Usuarios
                            {
                                Id= reader.GetInt32("Id"),
                                DNI = reader.GetString("DNI"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Telefono = reader.GetInt64("Telefono"),
                                Mail = reader.GetString("Mail"),

                            };
                            res.Add(r);
                        }
                        connection.Close();
                    }
                }
            }catch(Exception e){
                Console.WriteLine(e);
            }
            

            return res;
        }
    public Usuarios ObtenerXId(int id)
    {
        Usuarios res = null;
        try{
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = $"SELECT Id,DNI,Nombre,Apellido,Telefono,Mail FROM Usuarios WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new Usuarios
                        {
                                Id = reader.GetInt32("Id"),
                                DNI = reader.GetString("DNI"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Telefono = reader.GetInt64("Telefono"),
                                Mail = reader.GetString("Mail"),

                        };
                    }
                    connection.Close();
            }
        }
        }catch(Exception e){
            Console.WriteLine(e);
        }
        

            return res;

        }
    public int Alta(Usuarios u)
    {
        int res = -1;
        try{
            if(Existe(u)){
                throw new Exception("Ya existe este usuario");
            }
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
                string sql = "INSERT INTO Usuarios (Id,DNI,Nombre,Apellido,Telefono,Mail)"+
                            $"Values (@Id,@DNI,@Nombre,@Apellido,@Telefono,@Mail);"+
                            $"SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql,connection)){
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",u.Id);
                    command.Parameters.AddWithValue("@DNI",u.DNI);
                    command.Parameters.AddWithValue("@Nombre",u.Nombre);
                    command.Parameters.AddWithValue("@Apellido",u.Apellido);
                    command.Parameters.AddWithValue("@Telefono",u.Telefono);
                    command.Parameters.AddWithValue("@Mail",u.Mail);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    u.Id = res;
                    connection.Close();
                }
            }
        }catch(Exception e)
        {
            Console.Write(e);
            throw e;
        }
            
        return res;
    }
    public bool Baja(Usuarios u){
        bool res = false;
        try{
            if(!Existe(u)){
                throw new Exception($"No exite este usuario");
            }
            using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                string sql = $"DELETE FROM Usuarios WHERE Id = @Id;";
                using(MySqlCommand command = new MySqlCommand (sql,connection)){
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",u.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery() != 0;
                    connection.Close();
                }
            }
        }catch(Exception e)
        {
            Console.Write(e);
            throw e;

        }
            
        return res;
    }
    public bool Modificacion(int id,Usuarios u)
    {
        bool res = false;
        var x = ObtenerXId(id);
        try{
            if(!Existe(u)){
                throw new Exception($"No exite este usuario {u.Id}");
            }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                {
                    string sql = $"UPDATE Usuarios SET " +
                                $"DNI=@DNI,Nombre=@Nombre,Apellido=@Apellido,Telefono=@Telefono,Mail=@Mail"+
                                $" WHERE Id = @Id;";
                    using (MySqlCommand command = new MySqlCommand (sql,connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Id",u.Id);
                        command.Parameters.AddWithValue("@DNI",u.DNI);
                        command.Parameters.AddWithValue("@Nombre",u.Nombre);
                        command.Parameters.AddWithValue("@Apellido",u.Apellido);
                        command.Parameters.AddWithValue("@Telefono",u.Telefono);
                        command.Parameters.AddWithValue("@Mail",u.Mail);
                        connection.Open();
                        res = command.ExecuteNonQuery() != 0;
                        connection.Close();
                    }
            }
        }catch(Exception e){
            Console.WriteLine(e);
        }
        return res;
    }
    
    public UsuariosEspeciales ObtenerXMail(string mail){
        UsuariosEspeciales res = null;
        Usuarios Usuario = null;
        try{
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = $"SELECT Id,DNI,Nombre,Apellido,Telefono,Mail FROM Usuarios WHERE Mail = @Mail;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Mail",mail);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        Usuario = new Usuarios
                        {
                                Id = reader.GetInt32("Id"),
                                DNI = reader.GetString("DNI"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Telefono = reader.GetInt64("Telefono"),
                                Mail = reader.GetString("Mail"),

                        };
                    }
                    connection.Close();

                }
                if(Usuario != null)
                {
                    sql = $"SELECT Id,UsuarioId,Clave,Avatar FROM Administradores WHERE Id = @Id;";
                    using (MySqlCommand command= new MySqlCommand(sql,connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Id",Usuario.Id);
                        connection.Open();
                        var reader = command.ExecuteReader();
                        if(reader.Read())
                        {
                            res  = new Administradores
                            {
                                    Id = reader.GetInt32("Id"),
                                    UsuarioId = ObtenerXId(reader.GetInt32("UsuarioId")),
                                    Clave = reader.GetString("Clave"),
                                    rol = "Administrador"
                            };
                        }
                        connection.Close();
                        
                    }
                    if(res == null)
                    {
                       sql = $"SELECT Id,UsuarioId,Clave FROM Empleados WHERE Id = @Id;";
                        using (MySqlCommand command= new MySqlCommand(sql,connection))
                        {
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@Id",Usuario.Id);
                            connection.Open();
                            var reader = command.ExecuteReader();
                            if(reader.Read())
                            {
                                res  = new Empleados
                                {
                                        Id = reader.GetInt32("Id"),
                                        UsuarioId = ObtenerXId(reader.GetInt32("UsuarioId")),
                                        Clave = reader.GetString("Clave"),
                                        rol = "Empleado"
                                };
                            }
                            connection.Close();
                            
                        } 
                    }
                }else{
                    throw new Exception("Mail o Clave incorrecta");
                }
                
            }
            return res;
        }catch(Exception e){
            Console.WriteLine(e);
            throw e;
        }
        
    }
    
    public bool Existe(Usuarios u){
        return ObtenerXId(u.Id) != null;
    }
}
     
