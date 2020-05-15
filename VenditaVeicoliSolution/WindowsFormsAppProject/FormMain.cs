using System;
using System.Windows.Forms;
using carShopDllProject;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Threading;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WindowsFormsAppProject
{
    public partial class FormMain : Form
    {
        FolderBrowserDialog FolderBrowserDialog=new FolderBrowserDialog();
        dbUtils db = new dbUtils();
        SerializableBindingList<Veicolo> bindingListVeicoli;
        public bool modificato = false;

        public FormMain()
        {
            InitializeComponent();
            bindingListVeicoli = new SerializableBindingList<Veicolo>();
            listBoxVeicoli.DataSource = bindingListVeicoli;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            CaricaDati();
        }

        private void CaricaDati()
        {
            try
            {
                db.listaTabella("Auto", bindingListVeicoli);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            try
            {
                db.listaTabella("Moto", bindingListVeicoli);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void nuovoToolStripButton_Click(object sender, EventArgs e)
        {
            frmAggiungiVeicolo dialogAggiungi = new frmAggiungiVeicolo(bindingListVeicoli, this);
            dialogAggiungi.ShowDialog();
        }

        private void salvaToolStripButton_Click(object sender, EventArgs e)
        {
            salvataggioDatabase();
        }

        private void salvataggioDatabase()
        {
            int[] idDb = new int[db.contaItem("Auto")];
            db.getIds("Auto", idDb);
            SerializableBindingList<int> idList = new SerializableBindingList<int>();
            foreach (Auto auto in bindingListVeicoli.OfType<Auto>())
            {
                if (auto.Id == 0)
                {
                    db.aggiungiRecord("Auto", auto.Marca, auto.Modello, auto.Colore, auto.Cilindrata, auto.PotenzaKw, auto.Immatricolazione,
                        auto.IsUsato, auto.IsKmZero, auto.KmPercorsi, auto.Prezzo, auto.NumAirbag, "/");
                }
                else
                {
                    idList.Add(auto.Id);
                }
            }
            if (idList.Count != 0)
            {
                for (int i = 0; i < idDb.Length; i++)
                {
                    bool presente = false;
                    int j = 0;
                    do
                    {
                        if (idDb[i] == idList[j])
                            presente = true;
                        j++;
                    } while (j != idList.Count && presente == false);
                    if (presente == false)
                    {
                        db.eliminaRecord("Auto", idDb[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < idDb.Length; i++)
                    db.eliminaRecord("Auto", idDb[i]);
            }

            idList.Clear();
            idDb = new int[db.contaItem("Moto")];
            db.getIds("Moto", idDb);
            foreach (Moto moto in bindingListVeicoli.OfType<Moto>())
            {
                if (moto.Id == 0)
                {
                    db.aggiungiRecord("Moto", moto.Marca, moto.Modello, moto.Colore, moto.Cilindrata, moto.PotenzaKw, moto.Immatricolazione,
                        moto.IsUsato, moto.IsKmZero, moto.KmPercorsi, moto.Prezzo, 0, moto.MarcaSella);
                }
                else
                {
                    idList.Add(moto.Id);
                }
            }
            if (idList.Count != 0)
            {
                for (int i = 0; i < idList.Count; i++)
                {
                    bool presente = false;
                    int j = 0;
                    do
                    {
                        if (idList[i] == idDb[j])
                            presente = true;
                        j++;
                    } while (j != idDb.Length && presente == false);
                    if (presente == false)
                    {
                        db.eliminaRecord("Moto", idList[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < idDb.Length; i++)
                    db.eliminaRecord("Moto", idDb[i]);
            }

            modificato = false;
            bindingListVeicoli.Clear();
            CaricaDati();
            MessageBox.Show("Dati salvati correttamente");
        }

        private void stampaToolStripButton_Click(object sender, EventArgs e)
        {
            string webPath = (@"www\index.html");
            Utils.createHtml(bindingListVeicoli, webPath);
            System.Diagnostics.Process.Start(webPath);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listBoxVeicoli.SelectedIndex!=-1)
            {
                bindingListVeicoli.RemoveAt(listBoxVeicoli.SelectedIndex);
                modificato = true; 
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(modificato)
                if(MessageBox.Show("Vuoi salvare le modifiche prima di usicre?", "ATTENZIONE!!", MessageBoxButtons.YesNo)==DialogResult.Yes)
                salvataggioDatabase();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(FolderBrowserDialog.ShowDialog()==DialogResult.OK)
            {
                try
                {
                    string path = OutputFileName(FolderBrowserDialog.SelectedPath, "docx");
                    using (WordprocessingDocument doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = doc.AddMainDocumentPart();
                        mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                        Body body = mainPart.Document.AppendChild(new Body());

                        wordUtils.AddStyle(mainPart, true, true, false, false, "HeadingMain", "Default", "Garamond", 30, "FF0000");
                        wordUtils.AddStyle(mainPart, true, true, false, false, "HeadingAutoMoto", "Default", "Helvetica", 20, "3399FF");
                        wordUtils.AddStyle(mainPart, false, false, false, false, "Body", "Default", "Garamond", 25, "171717");
                        wordUtils.AddParagraph(body, "HeadingMain", "SALONE CAMOSCINI - AUTO E MOTO NUOVI E USATI", JustificationValues.Center);
                        wordUtils.AddParagraph(body, "HeadingAutoMoto", "AUTO", JustificationValues.Left);
                        int i = 11;
                        string[] veicoli = new string[bindingListVeicoli.OfType<Auto>().Count()*11+11];
                        bool[] bools = new bool[bindingListVeicoli.OfType<Auto>().Count() * 11 + 11];
                        JustificationValues[] just = new JustificationValues[bindingListVeicoli.OfType<Auto>().Count() * 11 + 11];
                        veicoli[0] = "Marca";
                        veicoli[1] = "Modello";
                        veicoli[2] = "Colore";
                        veicoli[3] = "Cilindrata";
                        veicoli[4] = "Potenza";
                        veicoli[5] = "Immatricolazione";
                        veicoli[6] = "Usato";
                        veicoli[7] = "Km zero";
                        veicoli[8] = "Chilometraggio";
                        veicoli[9] = "Prezzo";
                        veicoli[10] = "Numero airbag";
                        for (int j = 0; j < 11; j++)
                        {
                            bools[j] = true;
                            just[j] = JustificationValues.Center;
                        }
                        foreach (Auto auto in bindingListVeicoli.OfType<Auto>())
                        {
                            veicoli[i++] = auto.Marca.ToString();
                            veicoli[i++] = auto.Modello.ToString();
                            veicoli[i++] = auto.Colore.ToString();
                            veicoli[i++] = auto.Cilindrata.ToString();
                            veicoli[i++] = auto.PotenzaKw.ToString();
                            veicoli[i++] = auto.Immatricolazione.ToShortDateString();
                            veicoli[i++] = auto.IsUsato.ToString();
                            veicoli[i++] = auto.IsKmZero.ToString();
                            veicoli[i++] = auto.KmPercorsi.ToString();
                            veicoli[i++] = auto.Prezzo.ToString();
                            veicoli[i++] = auto.NumAirbag.ToString();
                        }
                        for (int y = 12; y < veicoli.Length; y++)
                        {
                            bools[y] = false;
                            just[y] = JustificationValues.Center;
                        }
                        Table tab=wordUtils.CreateTable(mainPart, bools, bools, bools, veicoli, just, bindingListVeicoli.OfType<Auto>().Count() + 1, 11, "171717", BorderValues.BabyRattle);
                        doc.MainDocumentPart.Document.Body.Append(tab);
                        wordUtils.AddParagraph(body, "HeadingAutoMoto", "MOTO", JustificationValues.Left);
                        veicoli = new string[bindingListVeicoli.OfType<Moto>().Count() * 11 + 11];
                        bools = new bool[bindingListVeicoli.OfType<Moto>().Count() * 11 + 11];
                        just = new JustificationValues[bindingListVeicoli.OfType<Moto>().Count() * 11 + 11];
                        i = 11;
                        veicoli[0] = "Marca";
                        veicoli[1] = "Modello";
                        veicoli[2] = "Colore";
                        veicoli[3] = "Cilindrata";
                        veicoli[4] = "Potenza";
                        veicoli[5] = "Immatricolazione";
                        veicoli[6] = "Usato";
                        veicoli[7] = "Km zero";
                        veicoli[8] = "Chilometraggio";
                        veicoli[9] = "Prezzo";
                        veicoli[10] = "Marca sella";
                        for (int j = 0; j < 11; j++)
                        {
                            bools[j] = true;
                            just[j] = JustificationValues.Center;
                        }
                        foreach (Moto moto in bindingListVeicoli.OfType<Moto>())
                        {
                            veicoli[i++] = moto.Marca.ToString();
                            veicoli[i++] = moto.Modello.ToString();
                            veicoli[i++] = moto.Colore.ToString();
                            veicoli[i++] = moto.Cilindrata.ToString();
                            veicoli[i++] = moto.PotenzaKw.ToString();
                            veicoli[i++] = moto.Immatricolazione.ToShortDateString();
                            veicoli[i++] = moto.IsUsato.ToString();
                            veicoli[i++] = moto.IsKmZero.ToString();
                            veicoli[i++] = moto.KmPercorsi.ToString();
                            veicoli[i++] = moto.Prezzo.ToString();
                            veicoli[i++] = moto.MarcaSella.ToString();
                        }
                        for (int y = 12; y < veicoli.Length; y++)
                        {
                            bools[y] = false;
                            just[y] = JustificationValues.Center;
                        }
                        tab = wordUtils.CreateTable(mainPart, bools, bools, bools, veicoli, just, bindingListVeicoli.OfType<Moto>().Count() + 1, 11, "171717", BorderValues.BabyRattle);
                        doc.MainDocumentPart.Document.Body.Append(tab);
                        if(MessageBox.Show("Documento creato correttamente, desideri visualizzarlo?","Creazione completata",MessageBoxButtons.YesNo)==DialogResult.Yes)
                            System.Diagnostics.Process.Start(path); 
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Impossibile aprire il documento, potrebbe essere aperto da un programma");
                }
            }
        }

        public string OutputFileName(string OutputFileDirectory, string fileExtension)
        {
            var datetime = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_");

            string fileFullname = Path.Combine(OutputFileDirectory, $"Output.{fileExtension}");

            if (File.Exists(fileFullname))
                fileFullname = Path.Combine(OutputFileDirectory, $"Output_{datetime}.{fileExtension}");

            return fileFullname;
        }
    }
}
