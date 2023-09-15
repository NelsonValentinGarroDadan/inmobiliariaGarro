namespace inmobiliaria.Models;

using System.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
public class TiposInmueblesRepositorio
{
        public List<TiposInmuebles> ObtenerTodos()
        {
            var res = new List<TiposInmuebles>();
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @"SELECT Id,Descripcion FROM TiposInmuebles;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    TiposInmuebles r;
                    while(reader.Read())
                    {
                        r = new TiposInmuebles
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
        public TiposInmuebles ObtenerXId(int id)
        {
            TiposInmuebles res = new TiposInmuebles{Id = -1, Descripcion = "Null"} ;
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @$"SELECT Id,Descripcion FROM TiposInmuebles WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new TiposInmuebles
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
        public bool Alta(TiposInmuebles te)
        {
            bool res = false;
            if(Existe(te)){
                throw new Exception("Ya exite este tipo de estado con id: "+te.Id);
            }
            using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
            string sql = "INSERT INTO TiposInmuebles (Id,Descripcion)"+
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
        public bool Baja(TiposInmuebles te){
            bool res = false;
            try{
                if(!Existe(te)){
                    throw new Exception("No exite este tipo de estado");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                    string sql = $"DELETE FROM TiposInmuebles WHERE Id = @Id;";
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
        public bool Modificacion(TiposInmuebles te)
        {
            bool res = false;
            try{
                if(!Existe(te)){
                        throw new Exception("No exite este tipo de estado");
                }
                    using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                    {
                        string sql = $"UPDATE TiposInmuebles SET " +
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
        private bool Existe(TiposInmuebles te){
            TiposInmuebles x = ObtenerXId(te.Id);
            return x.Id != -1 && x.Descripcion != "Null";
        }
}
