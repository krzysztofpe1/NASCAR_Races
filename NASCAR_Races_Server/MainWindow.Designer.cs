namespace NASCAR_Races
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            mainPictureBox = new PictureBox();
            programTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)mainPictureBox).BeginInit();
            SuspendLayout();
            // 
            // mainPictureBox
            // 
            mainPictureBox.Location = new Point(0, 0);
            mainPictureBox.Margin = new Padding(3, 4, 3, 4);
            mainPictureBox.Name = "mainPictureBox";
            mainPictureBox.Size = new Size(1352, 881);
            mainPictureBox.TabIndex = 0;
            mainPictureBox.TabStop = false;
            mainPictureBox.Paint += mainPictureBox_Paint;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1353, 881);
            Controls.Add(mainPictureBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainWindow";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)mainPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox mainPictureBox;
        private System.Windows.Forms.Timer programTimer;
    }
}