namespace inmobiliaria.Models;

using System.Data;
using MySql.Data.MySqlClient;
public class AdministradoresRepositorio
{
        public List<Administradores> ObtenerTodos()
        {
            var res = new List<Administradores>();
            var UR = new UsuariosRepositorio();
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @"SELECT Id,UsuarioId,Avatar FROM Administradores;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    Administradores admin;
                    while(reader.Read())
                    {
                        admin = new Administradores{
                            Id = reader.GetInt32("Id"),
                            UsuarioId = UR.ObtenerXId(reader.GetInt32("UsuarioId")),
                             Avatar = reader.IsDBNull("Avatar") ? null : reader.GetString("Avatar"),
                        };
                        res.Add(admin);
                    }
                    connection.Close();
                }
            }

            return res;
        }
        public Administradores ObtenerXId(int id)
        {
            Administradores res = null ;
            var UR = new UsuariosRepositorio();
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @$"SELECT Id,UsuarioId,Clave,Avatar FROM Administradores WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new Administradores{
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
        public bool Alta(Administradores A)
        {   
            bool res = false;
            var ER = new EmpleadosRepositorio();
            if(ER.Existe(new Empleados{Id = A.Id})){
                throw new Exception("Un administrador no puede ser Administrador y Empleado a la vez");
            }
            if(Existe(A)){
                throw new Exception("Ya exite este admin");
            }
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
                string sql = "INSERT INTO Administradores (Id,UsuarioId,Clave,Avatar)"+
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
        public bool Baja(Administradores A){
            bool res = false;
            try{
                if(!Existe(A)){
                    throw new Exception("No exite este admin");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                    string sql = $"DELETE FROM Administradores WHERE Id = @Id;";
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
        public bool CambiarClave(Administradores A)
        {
            bool res = false;
            try{
                if(!Existe(A)){
                        throw new Exception("No exite este admin");
                }
                    using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                    {
                        string sql = $"UPDATE Administradores SET " +
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
        public bool CambiarAvatar(Administradores A)
        {
            bool res = false;
            try{
                if(!Existe(A)){
                        throw new Exception("No exite este admin");
                }
                    using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                    {
                        string sql = $"UPDATE Administradores SET " +
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
        public bool Existe(Administradores A){
            var u = ObtenerXId(A.Id);
            return  u != null;
        }
}
