using System;
using Microsoft.VisualBasic;
using System.Data.OleDb;
using carShopDllProject;
using System.IO;

namespace ConsoleApp_Project
{
    class Program
    {

        public static dbUtils db = new dbUtils();
        public static string[] campi = { "Marca", "Modello", "Colore", "Cilindrata", "PotenzaKw", "Immatricolazione (GG/MM/AAAA)", "se usata (Si/No)", "se Km 0 (Si/No)", "KmPercorsi", "Prezzo" };
        static void Main(string[] args)
        {
            string[] newItem = new string[11];
            string[] listaVeicoli;
            string airbag;
            string nomeTab;
            int intParse;
            double douParse;
            char scelta;
            do
            {
                menu();
                Console.Write(" DIGITA LA TUA SCELTA: ");
                scelta = Console.ReadKey().KeyChar;
                switch (scelta)
                {
                    case '1':
                        nomeTab = sceltaTabella();
                        db.creaTabella(nomeTab); 
                        Console.WriteLine("Tabella creata correttamente");
                        Console.ReadKey();
                        break;
                    case '2':
                        nomeTab=sceltaTabella();
                        for (int i = 0; i < campi.Length; i++)
                        {
                            if(i==3 || i==8)
                            {
                                do
                                {
                                    Console.Write($"Inserisci {campi[i]}: ");
                                } while (!int.TryParse(Console.ReadLine(), out intParse));
                                newItem[i] = intParse.ToString();
                            }
                            else if(i==4 || i==9)
                            {
                                do
                                {
                                    Console.Write($"Inserisci {campi[i]}: ");
                                } while (!double.TryParse(Console.ReadLine(), out douParse));
                                newItem[i] = douParse.ToString();
                            }
                            else
                            {
                                Console.Write($"Inserisci {campi[i]}: ");
                                newItem[i] = Console.ReadLine();
                            }
                        }
                        if (nomeTab == "Auto")
                        {
                            Console.Write("Inserisci il numero di airbag: ");
                            airbag = Console.ReadLine();
                            newItem[10] = "/";
                        }
                        else
                        {
                            Console.Write("Inserisci la marca della sella: ");
                            newItem[10] = Console.ReadLine();
                            airbag = "0";
                        }
                        newItem[newItem.Length-1] = Console.ReadLine();
                        db.aggiungiRecord(nomeTab, newItem[0], newItem[1], newItem[2], Convert.ToInt32(newItem[3]), Convert.ToDouble(newItem[4]),
                            Convert.ToDateTime(newItem[5]), newItem[6]=="Si"? true:false, newItem[7] == "Si" ? true : false, Convert.ToInt32(newItem[8]),
                            Convert.ToDouble(newItem[9]), Convert.ToInt32(airbag), newItem[10]);
                        Console.WriteLine(nomeTab + " aggiunta correttamente");
                        Console.ReadKey();
                        break;
                    case '3':
                        nomeTab = sceltaTabella();
                        listaVeicoli = new string[db.contaItem(nomeTab)];
                        stampaVeicoli(nomeTab,listaVeicoli);
                        Console.ReadKey();
                        break;
                    case '4':
                        nomeTab = sceltaTabella();
                        listaVeicoli = new string[db.contaItem(nomeTab)];
                        stampaVeicoli(nomeTab, listaVeicoli);
                        do
                        {
                            Console.Write("Inserisci il numero del veicolo che desideri eliminare: ");
                        } while (!int.TryParse(Console.ReadLine(),out intParse));
                        db.eliminaRecord(nomeTab, Convert.ToInt32(listaVeicoli[intParse - 1].Split('-')[1])); //prendo L'id nascosto all'interno della stringa
                        Console.WriteLine(nomeTab+" eliminata correttamente");
                        Console.ReadKey();
                        break;
                    case '5':
                        nomeTab = sceltaTabella();
                        db.eliminaTabella(nomeTab);
                        Console.WriteLine("Tabella eliminata correttamente");
                        Console.ReadKey();
                        break;
                    default:
                        break;
                }
            } while (scelta != 'X' && scelta != 'x');
        }

        private static void stampaVeicoli(string nomeTab,string[] listaVeicoli)
        {
            if (db.listaTabella(nomeTab, listaVeicoli))
            {
                string[] hideID = new string[2];
                Console.WriteLine();
                for (int i = 0; i < listaVeicoli.Length; i++)
                {
                    hideID = listaVeicoli[i].Split('-'); //Escludo l'ID dalla stampa per rendere più intuitiva l'eventuale selezione di un veicolo attraverso la riga in cui si trova
                    Console.WriteLine((i + 1) + ".   " + hideID[0]);
                }
            }
            else
                Console.WriteLine("Nessun risultato trovato");
        }

        private static string sceltaTabella()
        {
            string nome;
            do
            {
                Console.Write("\nInserire nome tabella (Auto/Moto): ");
                nome = Console.ReadLine();
            } while (nome != "Auto" && nome != "Moto");
            return nome;
        }

        private static void menu()
        {
            Console.Clear();
            Console.WriteLine("*** CAR SHOP - DB MANAGEMENT ***\n");
            Console.WriteLine(" Menu:");
            Console.WriteLine(" 1 - CREA TABELLA");
            Console.WriteLine(" 2 - AGGIUNGI NUOVO ELEMENTO IN TABELLA A SCELTA");
            Console.WriteLine(" 3 - LISTA TABELLA A SCELTA");
            Console.WriteLine(" 4 - ELIMINA ELEMENTO DA TABELLA A SCELTA");
            Console.WriteLine(" 5 - ELIMINA TABELLA A SCELTA");
            Console.WriteLine("\n X - FINE LAVORO\n\n");
        }
    }
}
