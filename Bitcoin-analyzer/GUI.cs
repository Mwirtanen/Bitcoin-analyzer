using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bitcoin_analyzer
{
    /// <summary>
    /// This class handles the creation of the interface and interaction with it.
    /// </summary>
    
    public class GUI : Analyzer
    {
        public GUI() {}

        private Form newForm = new Form();                              // Form that contains the GUI.
        private DateTimePicker startDate = new DateTimePicker();        // Datetime picker for start date.
        private DateTimePicker endDate = new DateTimePicker();          // Datetime picker for end date.
        private Label startDateLabel = new Label();                     // Label for start date picker.
        private Label endDateLabel = new Label();                       // Label for end date picker.
        private Button analyzeButton = new Button();                    // Button to run the analysis.
        private TextBox resultsText = new TextBox();                    // Textbox for the analysis results.
        private Font smallFont = new Font("Arial", 10);                 // Font for start and end date labels.
        private Font bigFont = new Font("Arial", 12);                   // Font for textbox.


        // Creates and runs the interface for user.        
        public void DisplayGUI()
        {           
            newForm.Name = "Uncle Scrooge's bitcoin analyzer";
            newForm.Text = "Uncle Scrooge's bitcoin analyzer";
            newForm.Size = new Size(380, 400);
            newForm.StartPosition = FormStartPosition.CenterScreen;

            startDate.MaxDate = DateTime.Now;
            startDate.Format = DateTimePickerFormat.Short;
            startDate.Size = new Size(100, 20);
            startDate.Location = new Point(100, 30);
            newForm.Controls.Add(startDate);

            endDate.MaxDate = DateTime.Now;
            endDate.Format = DateTimePickerFormat.Short;
            endDate.Size = new Size(100, 20);
            endDate.Location = new Point(100, 70);
            newForm.Controls.Add(endDate);

            startDateLabel.Font = smallFont;
            startDateLabel.Text = "Start date";      
            startDateLabel.Size = new Size(100, 20);
            startDateLabel.Location = new Point(30, 30);
            newForm.Controls.Add(startDateLabel);

            endDateLabel.Font = smallFont;
            endDateLabel.Text = "End date";
            endDateLabel.Size = new Size(100, 20);
            endDateLabel.Location = new Point(30, 70);
            newForm.Controls.Add(endDateLabel);

            analyzeButton.Text = " Analyze!";
            analyzeButton.Size = new Size(100, 60);
            analyzeButton.Location = new Point(230, 30);
            analyzeButton.Click += new EventHandler(ShowResults);
            newForm.Controls.Add(analyzeButton);

            resultsText.Font = bigFont;
            resultsText.Multiline = true;
            resultsText.ReadOnly = true;
            resultsText.Size = new Size(300, 200);
            resultsText.Name = "Result Text";
            resultsText.Location = new Point(30, 120);
            newForm.Controls.Add(resultsText);
            
            Application.Run(newForm);
        }

        // Sends the data request and shows the results in the textbox.
        private void ShowResults(object source, EventArgs e)
        {
            Analyzer analysis = new Analyzer();
            analysis.HandleRequest(startDate.Value, endDate.Value);

            if(analysis.errors == null)
            {
                resultsText.Text = "Longest downward trend: " + analysis.numberOfDays + " days. \n";
                resultsText.AppendText("\n");
                resultsText.AppendText("Highest trading volume: " + analysis.highestVolumeAmount + "€ in " + analysis.highestVolumeDate.ToShortDateString() + "\n");
                resultsText.AppendText("\n");
                if (badTrend == false)
                {
                    resultsText.AppendText("Best day to buy: " + analysis.bestBuyDay.ToShortDateString() + "\n");
                    resultsText.AppendText("\n");
                    resultsText.AppendText("Best day to sell: " + analysis.bestSellDay.ToShortDateString() + "\n");
                }
                else
                {
                    resultsText.AppendText("No good days to buy or sell.");
                }
            }
            else
            {
                MessageBox.Show(analysis.errors);
                analysis.errors = null;
            }                                
        }
    }
}
       

