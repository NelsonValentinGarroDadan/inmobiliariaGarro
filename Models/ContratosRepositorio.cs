namespace inmobiliaria.Models;

using System.Data;
using MySql.Data.MySqlClient;
public class ContratosRepositorio
{
        public List<Contratos> ObtenerTodos()
        {
            var INR = new InquilinosRepositorio();
            var IR = new InmueblesRepositorio();
            var res = new List<Contratos>();
            try{
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
                {
                    string sql = @"SELECT Id,FechaInicio,FechaFin,InquilinoId,InmuebleId,Importe FROM Contratos;";
                    using (MySqlCommand command= new MySqlCommand(sql,connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        var reader = command.ExecuteReader();
                        Contratos c;
                        while(reader.Read())
                        {
                            c = new Contratos
                            {
                                Id = reader.GetInt32("Id"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                InquilinoId = INR.ObtenerXId(reader.GetInt32("InquilinoId")) ,
                                InmuebleId =IR.ObtenerXId(reader.GetInt32("InmuebleId")),
                                Importe = reader.GetDecimal("Importe"),

                            };
                            res.Add(c);
                        }
                        connection.Close();
                    }
            }
            }catch(Exception e){
                Console.WriteLine(e);
            }
            

            return res;
        }
        public Contratos ObtenerXId(int id)
        {
            var TER = new TiposEstadosRepositorio();
            var UR = new UsuariosRepositorio();
            var INR = new InquilinosRepositorio();
            var IR = new InmueblesRepositorio();
            Contratos res = null;
            try{
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @$"SELECT Id,FechaInicio,FechaFin,InquilinoId,InmuebleId,Importe FROM Contratos WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new Contratos
                        {
                            Id = reader.GetInt32("Id"),
                            FechaInicio = reader.GetDateTime("FechaInicio"),
                            FechaFin = reader.GetDateTime("FechaFin"),
                            InquilinoId = INR.ObtenerXId(reader.GetInt32("InquilinoId")) ,
                            InmuebleId = IR.ObtenerXId(reader.GetInt32("InmuebleId")),
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
        public int Alta(Contratos c)
        {
            int res = -1;
            try{
                if(Existe(c)){
                    throw new Exception("Ya existe este contrato");
                }
                if(c.InmuebleId.TipoEstadoId.Descripcion == "Ocupado"){
                    throw new Exception("El inmueble esta ocupado");
                }
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
                string sql = "INSERT INTO Contratos (FechaInicio,FechaFin,InquilinoId,InmuebleId,Importe)"+
                            $" Values (@FechaInicio,@FechaFin,@InquilinoId,@InmuebleId,@Importe);"+
                            $"SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql,connection)){
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@FechaInicio",c.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin",c.FechaFin);
                    command.Parameters.AddWithValue("@InquilinoId",c.InquilinoId.Id);
                    command.Parameters.AddWithValue("@InmuebleId",c.InmuebleId.Id);
                    command.Parameters.AddWithValue("@Importe",c.Importe);
                    connection.Open();
                    c.Id= Convert.ToInt32(command.ExecuteScalar());
                    res = c.Id;
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
        public bool Baja(Contratos c){
            bool res = false;
            try{
                if(!Existe(c)){
                    throw new Exception("No existe este inmueble");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                    string sql = $"DELETE FROM Contratos WHERE Id = @Id;";
                    using(MySqlCommand command = new MySqlCommand (sql,connection)){
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Id",c.Id);
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
        public bool Modificacion(int id,Contratos c){
            bool res = false;
            try{
                if(!Existe(c)){
                    throw new Exception("No existe este inmueble");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                        {
                            string sql = $"UPDATE Contratos SET " +
                                        $"FechaInicio=@FechaInicio,FechaFin=@FechaFin,InquilinoId=@InquilinoId,InmuebleId=@InmuebleId"+
                                        $" WHERE Id=@Id;";
                            using (MySqlCommand command = new MySqlCommand (sql,connection))
                            {
                                command.CommandType = CommandType.Text;
                                command.Parameters.AddWithValue("@Id",c.Id);
                                command.Parameters.AddWithValue("@FechaInicio",c.FechaInicio);
                                command.Parameters.AddWithValue("@FechaFin",c.FechaFin);
                                command.Parameters.AddWithValue("@InquilinoId",c.InquilinoId.Id);
                                command.Parameters.AddWithValue("@InmuebleId",c.InmuebleId.Id);
                                connection.Open();
                                res = command.ExecuteNonQuery() != 0;
                                connection.Close();
                            }
                        }
            }catch(Exception e){
                Console.WriteLine(e.Message);
                throw e;
            }
            return res;    
            
        
        }
        private bool Existe(Contratos c){
            return ObtenerXId(c.Id) != null;
        }
}
