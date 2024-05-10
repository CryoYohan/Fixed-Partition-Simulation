using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FixedPartitionSimulation
{
    public partial class Form1 : Form
    {
        private int timer = 0;
        private const int kernel = 50;
        private int RAM, processes; 
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer++;
            label4.Text = timer.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // Proceed to Table Button
        private void button1_Click(object sender, EventArgs e)
        {
            if (memoryRAMBox.Text == "" && noProcessesBox.Text == "")
                MessageBox.Show("Fields cannot be empty", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                int memoryRAM = Convert.ToInt32(memoryRAMBox.Text);
                int noProcesses = Convert.ToInt32(noProcessesBox.Text);
                processes = noProcesses;
                if((memoryRAM > 5000 || memoryRAM <= 0 || memoryRAM < 1000) || (noProcesses > 10 || noProcesses <= 0 || noProcesses < 5))
                    MessageBox.Show("Invalid input", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    dataGridView1.Show();
                    fillTable(noProcesses);
                    configureMemoryPartitions(memoryRAM, noProcesses);
                }
            }
            

        }
        // Allocate the Process ID's in the table
        private void fillTable(int processes)
        {
            for (int i = 0; i < processes; i++)
            {
                string processId = $"P{i + 1}"; // Generate process IDs (P1, P2, ...)
                dataGridView1.Rows.Add(processId); // Add a new row with the process ID
            }
        }
        private void configureMemoryPartitions(int memoryRAM, int noProcesses)
        {
            memoryRAMPanel.Margin = new Padding(0); // Set zero margin for no spacing
            memoryRAMPanel.Padding = new Padding(0);
            double panelHeight = memoryRAMPanel.Height;
            double panelWidth = memoryRAMPanel.Width;
            double heightOfPanels = panelHeight / noProcesses;
            double memoryUsable = memoryRAM - 50;
            decimal partitionSizes = (decimal)memoryUsable / (decimal)noProcesses;

            for (int i = 0; i < noProcesses; i++)
            {
                Panel panel = new Panel();
                panel.Size = new Size((int)panelWidth, (int)heightOfPanels);
                panel.BorderStyle = BorderStyle.FixedSingle;
                panel.Margin = new Padding(0);
                Label partitionLabel = new Label();
                partitionSizes = Math.Round(partitionSizes, 2);
                partitionLabel.Text = $"{partitionSizes} KB";
                partitionLabel.AutoSize = true;
                partitionLabel.Dock = DockStyle.Fill;
                panel.Controls.Add(partitionLabel);
                memoryRAMPanel.Controls.Add(panel);
            }
        }


        // Reset Computer Button to Reconfigure new Partitions for the Memory(RAM)
        private void button2_Click(object sender, EventArgs e)
        {
            memoryRAMPanel.Controls.Clear();
            dataGridView1.Rows.Clear();
            memoryRAMBox.Clear();
            noProcessesBox.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Hide();
        }
        // Generate Sequence of Fixed Partitioning Algorithm
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                int v = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    storeProcessesData(v, Convert.ToInt32(row.Cells[1].Value.ToString()), Convert.ToInt32(row.Cells[2].Value.ToString()), Convert.ToInt32(row.Cells[3].Value.ToString()));
                }
                v++;

            }
            MessageBox.Show(message, "Fixed Partition", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void storeProcessesData(int noProcesses, double memoryReq, int allocationTime, int completionTime)
        {
            int noElements = 5;
            double[,] dataTable = new double[noProcesses, noElements];
            for (int i = 0;  i < noProcesses; i++)
            {
                for(int k = 0; k < noElements; k++)
                {
                    switch (k)
                    {
                        case 0:
                            dataTable[i, k] = i+1; break;
                        case 1:
                            dataTable[i, k] = memoryReq; break;
                        case 2:
                            dataTable[i, k] = allocationTime; break;
                        case 3:
                            dataTable[i, k] = completionTime; break;
                    }
                }
            }
            printArray(dataTable);
        }
        string message = "";
        private void printArray(double[,]arr)
        {
           for(int i = 0; i < arr.GetLength(0); i++)
            {
                for(int k = 0; k < arr.GetLength(1); k++)
                {
                    message = arr[i, k] + "";
                }
                message = "\n";
            }
           
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
