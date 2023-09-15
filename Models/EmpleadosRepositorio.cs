namespace inmobiliaria.Models;

using System.Data;
using MySql.Data.MySqlClient;
public class EmpleadosRepositorio
{
        public List<Empleados> ObtenerTodos()
        {
            var res = new List<Empleados>();
            var UR = new UsuariosRepositorio();
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @"SELECT Id,UsuarioId,Avatar FROM Empleados;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    Empleados emp;
                    while(reader.Read())
                    {   emp = new Empleados{
                            Id = reader.GetInt32("Id"),
                            UsuarioId = UR.ObtenerXId(reader.GetInt32("UsuarioId")),
                            Avatar = reader.IsDBNull("Avatar") ? null : reader.GetString("Avatar"),
                        };
                        res.Add(emp);
                    }
                    connection.Close();
                }
            }

            return res;
        }
        public Empleados ObtenerXId(int id)
        {
            Empleados res = null ;
            var UR = new UsuariosRepositorio();
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @$"SELECT Id,UsuarioId,Clave,Avatar FROM Empleados WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new Empleados{
                            Id = reader.GetInt32("Id"),
                            UsuarioId = UR.ObtenerXId(reader.GetInt32("UsuarioId")),
                            Clave = reader.GetString("Clave"),
                            Avatar = reader.IsDBNull("Avatar") ? null : reader.GetString("Avatar"),
                        };
                    }
                    connection.Close();
                }
            }

            return res;

        }
        public bool Alta(Empleados A)
        {   
            bool res = false;
            var AR = new AdministradoresRepositorio();
            if(AR.Existe(new Administradores{Id = A.Id})){
                throw new Exception("Un Empleado no puede ser Empleado y Administrador a la vez");
            }
                if(Existe(A)){
                    throw new Exception("Ya exite este empleado");
                }
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
                string sql = "INSERT INTO Empleados (Id,UsuarioId,Clave,Avatar)"+
                            $"Values (@Id,@UsuarioId,@Clave,@Avatar);";
                using (MySqlCommand command = new MySqlCommand(sql,connection)){
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",A.Id);
                    command.Parameters.AddWithValue("@UsuarioId",A.Id);
                    command.Parameters.AddWithValue("@Clave",A.Clave);
                     if (String.IsNullOrEmpty(A.Avatar))
					{
                        command.Parameters.AddWithValue("@Avatar", DBNull.Value);
                    }
					else
					{
                        command.Parameters.AddWithValue("@Avatar", A.Avatar);
                    }
                    connection.Open();
                    command.ExecuteScalar();
                    res = A.Id != -1;
                    connection.Close();
                }
                }
            return res;
        }
        public bool Baja(Empleados A){
            bool res = false;
            try{
                if(!Existe(A)){
                    throw new Exception("No exite este admin");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                    string sql = $"DELETE FROM Empleados WHERE Id = @Id;";
                    using(MySqlCommand command = new MySqlCommand (sql,connection)){
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Id",A.Id);
                        connection.Open();
                        res = command.ExecuteNonQuery() != 0;
                        connection.Close();
                    }
                }
            }catch(Exception e)
            {
                Console.Write($"Ocurrio un erro al tratar de eliminar id:{A.Id}");
                throw e;

            }
            
            return res;
        }
        public bool CambiarClave(Empleados A)
        {
            bool res = false;
            try{
                if(!Existe(A)){
                        throw new Exception("No exite este admin");
                }
                    using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                    {
                        string sql = $"UPDATE Empleados SET " +
                                    $"Clave=@Clave WHERE Id = @Id;";
                        using (MySqlCommand command = new MySqlCommand (sql,connection))
                        {
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@Id",A.Id);
                            command.Parameters.AddWithValue("@Clave",A.Clave);
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
        public bool CambiarAvatar(Empleados A)
        {
            bool res = false;
            try{
                if(!Existe(A)){
                        throw new Exception("No exite este admin");
                }
                    using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                    {
                        string sql = $"UPDATE Empleados SET " +
                                    $"Avatar=@Avatar WHERE Id = @Id;";
                        using (MySqlCommand command = new MySqlCommand (sql,connection))
                        {
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@Id",A.Id);
                            command.Parameters.AddWithValue("@Avatar",A.Avatar);
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
  
        public bool Existe(Empleados A){
             var u = ObtenerXId(A.Id);
            return  u != null;
        }
}
