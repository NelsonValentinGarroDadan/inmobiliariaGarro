namespace inmobiliaria.Models;

using System.Data;
using MySql.Data.MySqlClient;
public class TiposUsosRepositorio
{
       public List<TiposUsos> ObtenerTodos()
        {
            var res = new List<TiposUsos>();
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @"SELECT Id,Descripcion FROM TiposUsos;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    TiposUsos r;
                    while(reader.Read())
                    {
                        r = new TiposUsos
                        {
                            Id = reader.GetInt32("Id"),
                            Descripcion = reader.GetString("Descripcion"),

                        };
                        res.Add(r);
                    }
                    connection.Close();
                }
            }

            return res;
        }
        public TiposUsos ObtenerXId(int id)
        {
            TiposUsos res = new TiposUsos{Id = -1, Descripcion = "Null"} ;
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @$"SELECT Id,Descripcion FROM TiposUsos WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new TiposUsos
                        {
                            Id = reader.GetInt32("Id"),
                            Descripcion = reader.GetString("Descripcion"),

                        };
                    }
                    connection.Close();
                }
            }

            return res;

        }
        public bool Alta(TiposUsos te)
        {
            bool res = false;
            if(Existe(te)){
                throw new Exception("Ya exite este tipo de estado con id: "+te.Id);
            }
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
            string sql = "INSERT INTO TiposUsos (Id,Descripcion)"+
                            $"Values (@Id,@Descripcion);"+
                            $"SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql,connection)){
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",te.Id);
                    command.Parameters.AddWithValue("@Descripcion",te.Descripcion);
                    connection.Open();
                    te.Id = Convert.ToInt32(command.ExecuteScalar());
                    res = te.Id != -1;
                    connection.Close();
                }
            }
            
            return res;
        }
        public bool Baja(TiposUsos te){
            bool res = false;
            try{
                if(!Existe(te)){
                    throw new Exception("No exite este tipo de estado");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                    string sql = $"DELETE FROM TiposUsos WHERE Id = @Id;";
                    using(MySqlCommand command = new MySqlCommand (sql,connection)){
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Id",te.Id);
                        connection.Open();
                        res = command.ExecuteNonQuery() != 0;
                        connection.Close();
                    }
                }
            }catch(Exception e)
            {
                Console.Write($"Ocurrio un erro al tratar de eliminar id:{te.Id}");
                throw e;

            }
            
            return res;
        }
        public bool Modificacion(TiposUsos te)
        {
            bool res = false;
            try{
                if(!Existe(te)){
                        throw new Exception("No exite este tipo de estado");
                }
                    using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                    {
                        string sql = $"UPDATE TiposUsos SET " +
                                    $"Descripcion=@Descripcion WHERE Id = @Id;";
                        using (MySqlCommand command = new MySqlCommand (sql,connection))
                        {
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@Id",te.Id);
                            command.Parameters.AddWithValue("@Descripcion",te.Descripcion);
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
        private bool Existe(TiposUsos te){
            return ObtenerXId(te.Id).Id != -1 && ObtenerXId(te.Id).Descripcion != "Null";
        }
}
