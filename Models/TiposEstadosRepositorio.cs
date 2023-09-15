namespace inmobiliaria.Models;

using System.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
public class TiposEstadosRepositorio
{
        public List<TiposEstados> ObtenerTodos()
        {
            var res = new List<TiposEstados>();
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @"SELECT Id,Descripcion FROM TiposEstados;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    TiposEstados r;
                    while(reader.Read())
                    {
                        r = new TiposEstados
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
        public TiposEstados ObtenerXId(int id)
        {
            TiposEstados res = new TiposEstados{Id = -1, Descripcion = "Null"} ;
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @$"SELECT Id,Descripcion FROM TiposEstados WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new TiposEstados
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
        public bool Alta(TiposEstados te)
        {
            bool res = false;
            if(Existe(te)){
                throw new Exception("Ya exite este tipo de estado con id: "+te.Id);
            }
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
            string sql = "INSERT INTO TiposEstados (Id,Descripcion)"+
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
        public bool Baja(TiposEstados te){
            bool res = false;
            try{
                if(!Existe(te)){
                    throw new Exception("No exite este tipo de estado");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                    string sql = $"DELETE FROM TiposEstados WHERE Id = @Id;";
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
        public bool Modificacion(TiposEstados te)
        {
            bool res = false;
            try{
                if(!Existe(te)){
                        throw new Exception("No exite este tipo de estado");
                }
                    using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                    {
                        string sql = $"UPDATE TiposEstados SET " +
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
        private bool Existe(TiposEstados te){
            TiposEstados x = ObtenerXId(te.Id);
            return x.Id != -1 && x.Descripcion != "Null";
        }
}
