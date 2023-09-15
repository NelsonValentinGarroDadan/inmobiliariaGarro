namespace inmobiliaria.Models;

using System.Data;
using MySql.Data.MySqlClient;public class InmueblesRepositorio
{
        public List<Inmuebles> ObtenerTodos()
        {
            var TUR = new TiposUsosRepositorio();
            var TER = new TiposEstadosRepositorio();
            var PR = new PropietariosRepositorio();
            var TIR = new TiposInmueblesRepositorio();
            var res = new List<Inmuebles>();
            try{
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
                {
                    string sql = @"SELECT Id,Direccion,TipoUsoId,CA,Longitud,Latitud,Precio,TipoEstadoId,PropietarioId,TipoInmuebleId,FechaInicio,FechaFin FROM Inmuebles;";
                    using (MySqlCommand command= new MySqlCommand(sql,connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        var reader = command.ExecuteReader();
                        Inmuebles r;
                        while(reader.Read())
                        {
                            r = new Inmuebles
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Longitud = reader.GetString("Longitud"),
                                Latitud = reader.GetString("Latitud"),
                                TipoUsoId = TUR.ObtenerXId(reader.GetInt32("TipoUsoId")),
                                CA = reader.GetInt32("CA"),
                                Precio = reader.GetDecimal("Precio"),
                                TipoEstadoId = TER.ObtenerXId(reader.GetInt32("TipoEstadoId")),
                                PropietarioId = PR.ObtenerXId(reader.GetInt32("PropietarioId")),
                                TipoInmuebleId = TIR.ObtenerXId(reader.GetInt32("TipoInmuebleId")),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),

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
        public Inmuebles ObtenerXId(int id)
        {
            var TUR = new TiposUsosRepositorio();
            var TER = new TiposEstadosRepositorio();
            var PR = new PropietariosRepositorio();
            var TIR = new TiposInmueblesRepositorio();
            Inmuebles res = null;
            try{
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection()))
            {
                string sql = @$"SELECT Id,Direccion,TipoUsoId,CA,Longitud,Latitud,Precio,TipoEstadoId,PropietarioId,TipoInmuebleId,FechaInicio,FechaFin FROM Inmuebles WHERE Id = @Id;";
                using (MySqlCommand command= new MySqlCommand(sql,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new Inmuebles
                        {
                            Id = reader.GetInt32("Id"),
                            Direccion = reader.GetString("Direccion"),
                            Longitud = reader.GetString("Longitud"),
                            Latitud = reader.GetString("Latitud"),
                            TipoUsoId = TUR.ObtenerXId(reader.GetInt32("TipoUsoId")),
                            CA = reader.GetInt32("CA"),
                            Precio = reader.GetDecimal("Precio"),
                            TipoEstadoId = TER.ObtenerXId(reader.GetInt32("TipoEstadoId")),
                            PropietarioId = PR.ObtenerXId(reader.GetInt32("PropietarioId")),
                            TipoInmuebleId = TIR.ObtenerXId(reader.GetInt32("TipoInmuebleId")),
                            FechaInicio = reader.GetDateTime("FechaInicio"),
                            FechaFin = reader.GetDateTime("FechaFin"),

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
        public int Alta(Inmuebles i)
        {
            int res = -1;
            try{
                if(Existe(i)){
                    throw new Exception("Ya existe este inmueble");
                }
                using(MySqlConnection connection = new MySqlConnection(Connection.stringConnection())){
                string sql = "INSERT INTO Inmuebles (Id,Direccion,TipoUsoId,CA,Longitud,Latitud,Precio,TipoEstadoId,PropietarioId,TipoInmuebleId,FechaInicio,FechaFin)"+
                            $"Values (@Id,@Direccion,@TipoUsoId,@CA,@Longitud,@Latitud,@Precio,@TipoEstadoId,@PropietarioId,@TipoInmuebleId,@FechaInicio,@FechaFin);"+
                            $"SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql,connection)){
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Id",i.Id);
                    command.Parameters.AddWithValue("@Direccion",i.Direccion);
                    command.Parameters.AddWithValue("@TipoUsoId",i.TipoUsoId.Id);
                    command.Parameters.AddWithValue("@CA",i.CA);
                    command.Parameters.AddWithValue("@Longitud",i.Longitud);
                    command.Parameters.AddWithValue("@Latitud",i.Latitud);
                    command.Parameters.AddWithValue("@Precio",i.Precio);
                    command.Parameters.AddWithValue("@TipoEstadoId",101);
                    command.Parameters.AddWithValue("@PropietarioId",i.PropietarioId.Id);
                    command.Parameters.AddWithValue("@TipoInmuebleId",i.TipoInmuebleId.Id);
                    command.Parameters.AddWithValue("@FechaInicio",i.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin",i.FechaFin);
                    connection.Open();
                    i.Id = Convert.ToInt32(command.ExecuteScalar());
                    res=i.Id;
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
        public bool Baja(Inmuebles i){
            bool res = false;
            try{
                if(!Existe(i)){
                    throw new Exception("No existe este inmueble");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection())){
                    string sql = $"DELETE FROM Inmuebles WHERE Id = @Id;";
                    using(MySqlCommand command = new MySqlCommand (sql,connection)){
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Id",i.Id);
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
        public bool Modificacion(int id,Inmuebles i){
            bool res = false;
                if(!Existe(i)){
                    throw new Exception("No existe este inmueble");
                }
                using (MySqlConnection connection = new MySqlConnection (Connection.stringConnection()))
                        {
                            string sql = $"UPDATE Inmuebles SET " +
                                        $"Direccion=@Direccion,TipoUsoId=@TipoUsoId,CA=@CA,Longitud=@Longitud,"+
                                        "Latitud=@Latitud,Precio=@Precio,TipoEstadoId=@TipoEstadoId,PropietarioId=@PropietarioId,"+
                                        "TipoInmuebleId=@TipoInmuebleId,FechaInicio=@FechaInicio,FechaFin=@FechaFin"+
                                        $" WHERE Id=@Id;";
                            using (MySqlCommand command = new MySqlCommand (sql,connection))
                            {
                                command.CommandType = CommandType.Text;
                                command.Parameters.AddWithValue("@Id",i.Id);
                                command.Parameters.AddWithValue("@Direccion",i.Direccion);
                                command.Parameters.AddWithValue("@TipoUsoId",i.TipoUsoId.Id);
                                command.Parameters.AddWithValue("@CA",i.CA);
                                command.Parameters.AddWithValue("@Longitud",i.Longitud);
                                command.Parameters.AddWithValue("@Latitud",i.Latitud);
                                command.Parameters.AddWithValue("@Precio",i.Precio);
                                command.Parameters.AddWithValue("@TipoEstadoId",i.TipoEstadoId.Id);
                                command.Parameters.AddWithValue("@PropietarioId",i.PropietarioId.Id);
                                command.Parameters.AddWithValue("@TipoInmuebleId",i.TipoInmuebleId.Id);
                                command.Parameters.AddWithValue("@FechaInicio",i.FechaInicio);
                                command.Parameters.AddWithValue("@FechaFin",i.FechaFin);
                                connection.Open();
                                res = command.ExecuteNonQuery() != 0;
                                connection.Close();
                            }
                        }  
            
            return res;    
            
        
        }

        public bool Deshabilitar(Inmuebles i){
            i.TipoEstadoId.Id= 102;
            i.FechaFin = DateTime.MinValue;
            i.FechaInicio = DateTime.MinValue;
            return Modificacion(0,i);
        }
        private bool Existe(Inmuebles i){
            return ObtenerXId(i.Id) != null;
        }
}
