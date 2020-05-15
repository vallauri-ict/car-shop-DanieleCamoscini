using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using carShopDllProject;

namespace WindowsFormsAppProject
{
    public partial class frmAggiungiVeicolo : Form
    {
        string color, veicolo;
        FormMain parent;
        SerializableBindingList<Veicolo> lista;
        public frmAggiungiVeicolo()
        {
            InitializeComponent();
        }

        public frmAggiungiVeicolo(SerializableBindingList<Veicolo> bindListaVeicolo, FormMain f)
        {
            InitializeComponent();
            lista = bindListaVeicolo;
            parent = f;
        }

        private void btnAnnulla_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAggiungiVeicolo_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (veicolo != null)
            {
                if(controlloCampi())
                {
                    if (veicolo == "Auto")
                    {
                        Auto a = new Auto(txtMarca.Text, txtModello.Text, color, Convert.ToInt32(nupCilindrata.Value), Convert.ToDouble(nupPotenza.Value), dtpDataImmatricolazione.Value, rdbNo.Checked ? false : true, cmbKm0.SelectedIndex == 0 ? true : false, Convert.ToInt32(nupKm.Value), Convert.ToDouble(numPrezzo.Value), Convert.ToInt32(nupNAirbag.Value),0);
                        lista.Add(a);
                        pulisciCampi();
                        aggioraCampi(cmbVeicolo.Text);
                    }
                    else
                    {
                        Moto m = new Moto(txtMarca.Text, txtModello.Text, color, Convert.ToInt32(nupCilindrata.Value), Convert.ToDouble(nupPotenza.Value), dtpDataImmatricolazione.Value, rdbNo.Checked ? false : true, cmbKm0.SelectedIndex == 0 ? true : false, Convert.ToInt32(nupKm.Value), Convert.ToDouble(numPrezzo.Value), txtMarcaSella.Text,0);
                        lista.Add(m);
                        pulisciCampi();
                        aggioraCampi(cmbVeicolo.Text);
                    }
                    parent.modificato = true;
                }
            }
            else
            {
                if (pnlControlli.Visible == true)
                    MessageBox.Show("Compilare prima i campi!!");
                else
                    errorProvider1.SetError(cmbVeicolo, "Selezionare un opzione!!!");
            }
        }

        private bool controlloCampi()
        {
            bool corretto = true;
            if (txtMarca.Text == "")
            {
                errorProvider1.SetError(txtMarca, "Compila il campo");
                corretto = false;
            }
            if (txtModello.Text == "")
            {
                errorProvider1.SetError(txtModello, "Compila il campo");
                corretto = false;
            }
            if (color == "")
            {
                errorProvider1.SetError(btnSelectColor, "Compila il campo");
                corretto = false;
            }
            if (nupCilindrata.Value == 0)
            {
                errorProvider1.SetError(nupCilindrata, "Compila il campo");
                corretto = false;
            }
            if (nupPotenza.Value == 0)
            {
                errorProvider1.SetError(nupPotenza, "Compila il campo");
                corretto = false;
            }
            if (dtpDataImmatricolazione.Value>DateTime.Now)
            {
                errorProvider1.SetError(dtpDataImmatricolazione, "Compila il campo");
                corretto = false;
            }
            if (!rdbNo.Checked && !rdbSi.Checked)
            {
                errorProvider1.SetError(rdbNo, "Compila il campo");
                corretto = false;
            }
            if (rdbNo.Checked && cmbKm0.SelectedIndex==-1)
            {
                errorProvider1.SetError(cmbKm0, "Compila il campo");
                corretto = false;
            }
            if (nupKm.Value==0)
            {
                errorProvider1.SetError(nupKm, "Compila il campo");
                corretto = false;
            }
            if (numPrezzo.Value==0)
            {
                errorProvider1.SetError(numPrezzo, "Compila il campo");
                corretto = false;
            }
            if (nupNAirbag.Value==0 && txtMarcaSella.Text=="")
            {
                errorProvider1.SetError(nupNAirbag, "Compila il campo");
                corretto = false;
            }
            return corretto;
        }

        private void btnSelectColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                color = colorDialog1.Color.Name.ToString();
        }

        private void CmbVeicolo_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlControlli.Visible = true;
            pulisciCampi();
            aggioraCampi(cmbVeicolo.Text);
            btnAggiungiVeicolo.Enabled = true;
        }

        private void pulisciCampi()
        {
            txtMarca.Text = "";
            txtModello.Text = "";
            color = "";
            nupCilindrata.Value = 0;
            nupKm.Value = 0;
            nupPotenza.Value = 0;
            rdbSi.Checked = false;
            rdbNo.Checked = false;
            nupNAirbag.Value = 0;
            nupNAirbag.Enabled = false;
            txtMarcaSella.Text = "";
            txtMarcaSella.Enabled = false;
            cmbKm0.SelectedIndex = -1;
            veicolo = "";
        }

        private void rdbNo_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbNo.Checked)
                cmbKm0.Enabled = true;
            else
                cmbKm0.Enabled = false;
        }

        private void rdbSi_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbSi.Checked)
                cmbKm0.Enabled = false;
            else
                cmbKm0.Enabled = true;
        }

        private void frmAggiungiVeicolo_Load(object sender, EventArgs e)
        {

        }

        private void aggioraCampi(string type)
        {
            switch (type)
            {
                case "Auto":
                    nupNAirbag.Enabled = true;
                    veicolo = "Auto";
                    break;
                case "Moto":
                    txtMarcaSella.Enabled = true;
                    veicolo = "Moto";
                    break;
            }
        }
    }
}
