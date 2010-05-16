namespace Cassia.Tests
{
    partial class TestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            this.rdpControl = new AxMSTSCLib.AxMsRdpClient7NotSafeForScripting();
            ((System.ComponentModel.ISupportInitialize)(this.rdpControl)).BeginInit();
            this.SuspendLayout();
            // 
            // rdpControl
            // 
            this.rdpControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdpControl.Enabled = true;
            this.rdpControl.Location = new System.Drawing.Point(0, 0);
            this.rdpControl.Name = "rdpControl";
            this.rdpControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("rdpControl.OcxState")));
            this.rdpControl.Size = new System.Drawing.Size(585, 418);
            this.rdpControl.TabIndex = 0;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 418);
            this.Controls.Add(this.rdpControl);
            this.Name = "TestForm";
            this.ShowIcon = false;
            this.Text = "Test RDP Connection";
            ((System.ComponentModel.ISupportInitialize)(this.rdpControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public AxMSTSCLib.AxMsRdpClient7NotSafeForScripting rdpControl;

    }
}