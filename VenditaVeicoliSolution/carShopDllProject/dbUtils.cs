using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;

namespace carShopDllProject
{
    public class dbUtils
    {
        public string conStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\CarShop.accdb";

        public void creaTabella(string nomeTabella)
        {
            if (conStr != null)
            {
                OleDbConnection con = new OleDbConnection(conStr);
                using (con)
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;

                    try
                    {
                        string command = $@"CREATE TABLE {nomeTabella}(id INT identity(1,1) NOT NULL PRIMARY KEY, marca VARCHAR(255) NOT NULL, modello VARCHAR(255) NOT NULL,
                                colore VARCHAR(255), cilindrata INT, potenzaKw INT,
                                immatricolazione DATE, usato VARCHAR(255), kmZero VARCHAR(255),
                                kmPercorsi INT, prezzo INT,";
                        if (nomeTabella == "Auto")
                            command += " numAirbag INT)";
                        else
                            command += " marcaSella VARCHAR(255))";
                        cmd.CommandText = command;
                        cmd.ExecuteNonQuery();
                    }
                    catch (OleDbException ex)
                    {
                        Console.WriteLine($"\n\n{ex.Message}");
                        System.Threading.Thread.Sleep(3000);
                        return;
                    }
                }
            }
        }

        public void aggiungiRecord(string nomeTabella, string marca, string modello, string colore, int cilindrata,
            double potenzaKw, DateTime immatricolazione, bool usata, bool isKm0, int kmPercorsi, double prezzo, int numAirbag, string marcaSella)
        {
            if (conStr != null)
            {
                OleDbConnection con = new OleDbConnection(conStr);
                using (con)
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;
                    string command = string.Empty;
                    if (nomeTabella == "Auto")
                        command = $"INSERT INTO {nomeTabella}(marca, modello, colore, cilindrata, potenzaKw, immatricolazione, usato, kmZero, kmPercorsi, prezzo, numAirbag) VALUES(@marca, @modello, @colore, @cilindrata, @potenzaKw, @immatricolazione, @usata, @isKm0, @kmPercorsi, @prezzo, @numAirbag)";
                    else
                        command = $"INSERT INTO {nomeTabella}(marca, modello, colore, cilindrata, potenzaKw, immatricolazione, usato, kmZero, kmPercorsi, prezzo, marcaSella) VALUES(@marca, @modello, @colore, @cilindrata, @potenzaKw, @immatricolazione, @usata, @isKm0, @kmPercorsi, @prezzo, @marcaSella)";
                    cmd.CommandText = command;

                    string used = usata ? "Si" : "No";
                    string km0 = isKm0 ? "Si" : "No";
                    cmd.Parameters.Add(new OleDbParameter("@marca", OleDbType.VarChar, 255)).Value = marca;
                    cmd.Parameters.Add(new OleDbParameter("@modello", OleDbType.VarChar, 255)).Value = modello;
                    cmd.Parameters.Add(new OleDbParameter("@colore", OleDbType.VarChar, 255)).Value = colore;
                    cmd.Parameters.Add("@cilindrata", OleDbType.Integer).Value = cilindrata;
                    cmd.Parameters.Add("@potenzaKw", OleDbType.Integer).Value = potenzaKw;
                    cmd.Parameters.Add(new OleDbParameter("@immatricolazione", OleDbType.Date)).Value = immatricolazione.ToShortDateString();
                    cmd.Parameters.Add(new OleDbParameter("@usato", OleDbType.VarChar, 255)).Value = used;
                    cmd.Parameters.Add(new OleDbParameter("@isKm0", OleDbType.VarChar, 255)).Value = km0;
                    cmd.Parameters.Add("@kmPercorsi", OleDbType.Integer).Value = kmPercorsi;
                    cmd.Parameters.Add("@prezzo", OleDbType.Double).Value = prezzo;
                    if (nomeTabella == "Auto")
                        cmd.Parameters.Add("@numAirbag", OleDbType.Integer).Value = numAirbag;
                    else
                        cmd.Parameters.Add(new OleDbParameter("@marcaSella", OleDbType.VarChar, 255)).Value = marcaSella;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void getIds(string tabName, int[] v)
        {
            OleDbConnection con = new OleDbConnection(conStr);
            if (conStr != null)
            {
                using (con)
                {
                    con.Open();
                    OleDbCommand cmd = new OleDbCommand($"SELECT id FROM {tabName}", con);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        int i = 0;
                        while (reader.Read())
                        {
                            v[i++] = reader.GetInt32(0);
                        }
                    }
                    reader.Close();
                }
            }
        }

        public bool listaTabella(string nomeTabella, string[] lista)
        {
            if (conStr != null)
            {
                OleDbConnection con = new OleDbConnection(conStr);
                using (con)
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand($"SELECT * FROM {nomeTabella}", con);

                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        int i = 0;
                        while (reader.Read())
                        {
                            string immatricolazione = reader.GetDateTime(6).ToShortDateString();
                            string boolVal = reader.GetString(7) == "Si" ? "usato |" : "non usato |";
                            boolVal += reader.GetString(8) == "Si" ? " km zero" : " non km zero";

                            lista[i] = $"{reader.GetString(1)} | {reader.GetString(2)} | " +
                                            $"{reader.GetString(3)} | {reader.GetInt32(4)} | {reader.GetInt32(5)} | " +
                                            $"{immatricolazione} | {boolVal} | {reader.GetInt32(9)} | " +
                                            $"{reader.GetInt32(10)}";
                            if (nomeTabella == "Auto")
                                lista[i] += $" | {reader.GetInt32(11)}";
                            else
                                lista[i] += $" | {reader.GetString(11)}";
                            lista[i++] += $"-{reader.GetInt32(0)}"; //Nascondo l'ID alla fine della stringa
                        }
                        reader.Close();
                        return true;
                    }
                    else
                    {
                        reader.Close();
                        return false;
                    }
                }
            }
            return false;
        }

        public bool listaTabella(string nomeTabella, SerializableBindingList<Veicolo> bindingVeicolo)
        {
            if (conStr != null)
            {
                OleDbConnection con = new OleDbConnection(conStr);
                using (con)
                {
                    con.Open();
                    OleDbCommand cmd;
                    try
                    {
                        cmd = new OleDbCommand($"SELECT * FROM {nomeTabella}", con);

                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (nomeTabella == "Auto")
                            {
                                Auto a = new Auto(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5),
                                    reader.GetDateTime(6), reader.GetString(7) == "Si" ? true:false, reader.GetString(8) == "Si" ? true:false, reader.GetInt32(9), reader.GetInt32(10),reader.GetInt32(11), reader.GetInt32(0));
                                bindingVeicolo.Add(a);
                            }
                            else
                            {
                                Moto m = new Moto(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5),
                                    reader.GetDateTime(6), reader.GetString(7) == "Si" ? true : false, reader.GetString(8) == "Si" ? true : false, reader.GetInt32(9), reader.GetInt32(10), reader.GetString(11), reader.GetInt32(0));
                                bindingVeicolo.Add(m);
                            }
                        }
                        reader.Close();
                        return true;
                    }
                    else
                    {
                        reader.Close();
                        return false;
                    }
                }
            }
            return false;
        }

        public void eliminaRecord(string tableName, int id)
        {
            if (conStr != null)
            {
                OleDbConnection con = new OleDbConnection(conStr);
                using (con)
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand($"DELETE FROM {tableName} WHERE id={id}", con);

                    cmd.Prepare();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void eliminaTabella(string tableName)
        {
            if (conStr != null)
            {
                OleDbConnection con = new OleDbConnection(conStr);
                using (con)
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;
                    string command = $"DROP TABLE {tableName}";
                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int contaItem(string tableName)
        {
            if (conStr != null)
            {
                OleDbConnection con = new OleDbConnection(conStr);

                using (con)
                {
                    con.Open();
                    OleDbCommand command = new OleDbCommand($"SELECT COUNT(*) FROM {tableName}", con);
                    OleDbDataReader rdr = command.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        return rdr.GetInt32(0);
                    }
                    else Console.WriteLine("\n\nNo rows found.");
                    rdr.Close();
                }
            }
            return -1;
        }

    }
}
