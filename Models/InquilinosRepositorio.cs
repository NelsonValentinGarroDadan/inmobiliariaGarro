namespace inmobiliaria.Models;

using System.Data;
using MySql.Data.MySqlClient;
public class InquilinosRepositorio
{
        public List<Inquilinos> ObtenerTodos()
        {
            var res = new List<Inquilinos>();
            var UR = new UsuariosRepositorio();
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @"SELECT Id,UsuarioId FROM Inquilinos;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    Inquilinos admin;
                    while(reader.Read())
                    {
                        admin = new Inquilinos{
                            Id = reader.GetInt32("Id"),
                            UsuarioId = UR.ObtenerXId(reader.GetInt32("UsuarioId")),
                        };
                        res.Add(admin);
                    }
                    connection.Close();
                }
            }

            return res;
        }
        public Inquilinos ObtenerXId(int id)
        {
            Inquilinos res = null ;
            var UR = new UsuariosRepositorio();
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @$"SELECT Id,UsuarioId FROM Inquilinos WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new Inquilinos{
                            Id = reader.GetInt32("Id"),
                            UsuarioId = UR.ObtenerXId(reader.GetInt32("UsuarioId")),
                        };
                    }
                    connection.Close();
                }
            }

            return res;

        }
        public bool Alta(Inquilinos A)
        {   
            bool res = false;
            
             if(Existe(A)){
                throw new Exception("Ya exite este propietario");
            }
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
                string sql = "INSERT INTO Inquilinos (Id,UsuarioId)"+
                            $"Values (@Id,@UsuarioId);";
                using (MySqlCommand command = new MySqlCommand(sql,connection)){
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",A.Id);
                    command.Parameters.AddWithValue("@UsuarioId",A.Id);
                    connection.Open();
                    command.ExecuteScalar();
                    res = A.Id != -1;
                    connection.Close();
                }
            }
            
            return res;
        }
        public bool Baja(Inquilinos A){
            bool res = false;
            try{
                if(!Existe(A)){
                    throw new Exception("No exite este admin");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                    string sql = $"DELETE FROM Inquilinos WHERE Id = @Id;";
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
      
        private bool Existe(Inquilinos A){
            var u = ObtenerXId(A.Id);
            return  u != null;
        }
}
