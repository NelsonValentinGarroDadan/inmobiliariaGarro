namespace inmobiliaria.Models;

using System.Data;
using MySql.Data.MySqlClient;
public class PagosRepositorio
{
          public List<Pagos> ObtenerTodos()
        {
            var CR = new ContratosRepositorio();
            var res = new List<Pagos>();
            try{
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
                {
                    string sql = @"SELECT Id,Fecha,ContratoId,Importe FROM Pagos;";
                    using (MySqlCommand command= new MySqlCommand(sql,connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        var reader = command.ExecuteReader();
                        while(reader.Read())
                        {
                            Pagos r = new Pagos
                            {
                                Id = reader.GetInt32("Id"),
                                Fecha = reader.GetDateTime("Fecha"),
                                ContratoId = CR.ObtenerXId(reader.GetInt32("ContratoId")),
                                Importe = reader.GetDecimal("Importe"),

                            };
                            res.Add(r);
                        }
                        connection.Close();
                    }
            }
            }catch(Exception e){
                Console.WriteLine(e.Message);
                throw e;
            }
            

            return res;
        }
        public Pagos ObtenerXId(int id)
        {
            var CR = new ContratosRepositorio();
            Pagos res = null;
            try{
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @$"SELECT Id,Fecha,ContratoId,Importe FROM Pagos WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new Pagos
                        {
                            Id = reader.GetInt32("Id"),
                            Fecha = reader.GetDateTime("Fecha"),
                            ContratoId = CR.ObtenerXId(reader.GetInt32("ContratoId")),
                            Importe = reader.GetDecimal("Importe"),

                        };
                    }
                    connection.Close();
                }
            }
            }catch(Exception e){
                Console.WriteLine(e);

                throw e;
            }
            

            return res;

        }
        public int Alta(Pagos p)
        {
            int res = -1;
            try{
                if(Existe(p)){
                    throw new Exception("Ya existe este pago");
                }
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
                string sql = "INSERT INTO Pagos (Id,Fecha,ContratoId,Importe)"+
                            $"Values (@Id,@Fecha,@ContratoId,@Importe);"+
                            $"SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql,connection)){
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",p.Id);
                    command.Parameters.AddWithValue("@Fecha",p.Fecha);
                    command.Parameters.AddWithValue("@ContratoId",p.ContratoId.Id);
                    command.Parameters.AddWithValue("@Importe",p.Importe);
                    connection.Open();
                    p.Id = Convert.ToInt32(command.ExecuteScalar());
                    res = p.Id;
                    connection.Close();
                }
            }
            }catch(Exception e)
            {
                Console.Write(e.Message);
                throw e;
            }
            
            return res;
        }
        public bool Baja(Pagos p){
            bool res = false;
            try{
                if(!Existe(p)){
                    throw new Exception("No existe este inmueble");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                    string sql = $"DELETE FROM Pagos WHERE Id = @Id;";
                    using(MySqlCommand command = new MySqlCommand (sql,connection)){
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Id",p.Id);
                        connection.Open();
                        res = command.ExecuteNonQuery() != 0;
                        connection.Close();
                    }
                }
            }catch(Exception e)
            {
                Console.Write(e.Message);
                throw e;

            }
            
            return res;
        }
        public bool Modificacion(int id,Pagos p){
            bool res = false;
            try{
                if(!Existe(p)){
                    throw new Exception("No existe este inmueble");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                        {
                            string sql = $"UPDATE Pagos SET " +
                                        $"Fecha=@Fecha,ContratoId=@ContratoId,Importe=@Importe"+
                                        $" WHERE Id=@Id";
                            using (MySqlCommand command = new MySqlCommand (sql,connection))
                            {
                                command.CommandType = CommandType.Text;
                                command.Parameters.AddWithValue("@Id",p.Id);
                                command.Parameters.AddWithValue("@Fecha",p.Fecha);
                                command.Parameters.AddWithValue("@ContratoId",p.ContratoId.Id);
                                command.Parameters.AddWithValue("@Importe",p.Importe);
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
        private bool Existe(Pagos p){
            return ObtenerXId(p.Id) != null;
        }
}