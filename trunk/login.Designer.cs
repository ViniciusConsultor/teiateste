namespace uniBaterFrenteLoja
{
    partial class login
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(login));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbLojas = new System.Windows.Forms.ComboBox();
            this.btnEntrar = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSenha = new System.Windows.Forms.MaskedTextBox();
            this.flwModulos = new System.Windows.Forms.FlowLayoutPanel();
            this.imgCupomFiscal = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblModulos = new System.Windows.Forms.Label();
            this.flwModulos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgCupomFiscal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 282);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(479, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(16, 145);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(193, 20);
            this.txtUsuario.TabIndex = 1;
            this.txtUsuario.Text = "chip";
            this.txtUsuario.TextChanged += new System.EventHandler(this.txtUsuario_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(118)))), ((int)(((byte)(188)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(17, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 51;
            this.label4.Text = "USUÁRIO:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(118)))), ((int)(((byte)(188)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(17, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 51;
            this.label2.Text = "LOJA:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Red;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(17, 171);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 51;
            this.label1.Text = "SENHA:";
            // 
            // cbLojas
            // 
            this.cbLojas.DropDownHeight = 300;
            this.cbLojas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbLojas.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbLojas.FormattingEnabled = true;
            this.cbLojas.IntegralHeight = false;
            this.cbLojas.Location = new System.Drawing.Point(16, 99);
            this.cbLojas.Name = "cbLojas";
            this.cbLojas.Size = new System.Drawing.Size(193, 24);
            this.cbLojas.TabIndex = 0;
            // 
            // btnEntrar
            // 
            this.btnEntrar.BackColor = System.Drawing.Color.Gray;
            this.btnEntrar.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnEntrar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(118)))), ((int)(((byte)(188)))));
            this.btnEntrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEntrar.ForeColor = System.Drawing.Color.White;
            this.btnEntrar.Location = new System.Drawing.Point(109, 217);
            this.btnEntrar.Name = "btnEntrar";
            this.btnEntrar.Size = new System.Drawing.Size(98, 25);
            this.btnEntrar.TabIndex = 3;
            this.btnEntrar.Text = "ENTRAR";
            this.btnEntrar.UseVisualStyleBackColor = false;
            this.btnEntrar.Click += new System.EventHandler(this.btnEntrar_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(62, 259);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(136, 13);
            this.lblStatus.TabIndex = 207;
            this.lblStatus.Text = "AGUARDANDO LOGIN";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(4, 259);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 207;
            this.label5.Text = "STATUS:";
            // 
            // txtSenha
            // 
            this.txtSenha.Font = new System.Drawing.Font("Wingdings", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSenha.ForeColor = System.Drawing.Color.Red;
            this.txtSenha.Location = new System.Drawing.Point(16, 188);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.PasswordChar = '«';
            this.txtSenha.Size = new System.Drawing.Size(191, 22);
            this.txtSenha.TabIndex = 2;
            this.txtSenha.Text = "master";
            this.txtSenha.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSenha_KeyPress);
            // 
            // flwModulos
            // 
            this.flwModulos.AutoScroll = true;
            this.flwModulos.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flwModulos.Controls.Add(this.imgCupomFiscal);
            this.flwModulos.Controls.Add(this.pictureBox4);
            this.flwModulos.Controls.Add(this.pictureBox5);
            this.flwModulos.Location = new System.Drawing.Point(228, 99);
            this.flwModulos.Name = "flwModulos";
            this.flwModulos.Size = new System.Drawing.Size(238, 169);
            this.flwModulos.TabIndex = 209;
            // 
            // imgCupomFiscal
            // 
            this.imgCupomFiscal.Image = global::uniBaterFrenteLoja.Properties.Resources.btnCupomFiscal;
            this.imgCupomFiscal.Location = new System.Drawing.Point(0, 0);
            this.imgCupomFiscal.Margin = new System.Windows.Forms.Padding(0);
            this.imgCupomFiscal.Name = "imgCupomFiscal";
            this.imgCupomFiscal.Padding = new System.Windows.Forms.Padding(5);
            this.imgCupomFiscal.Size = new System.Drawing.Size(69, 70);
            this.imgCupomFiscal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgCupomFiscal.TabIndex = 0;
            this.imgCupomFiscal.TabStop = false;
            this.toolTip1.SetToolTip(this.imgCupomFiscal, "Venda com cupom fiscal.");
            this.imgCupomFiscal.MouseLeave += new System.EventHandler(this.imgCupomFiscal_MouseLeave);
            this.imgCupomFiscal.Click += new System.EventHandler(this.imgCupomFiscal_Click);
            this.imgCupomFiscal.MouseHover += new System.EventHandler(this.imgCupomFiscal_MouseHover);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::uniBaterFrenteLoja.Properties.Resources.btnNotaFiscal;
            this.pictureBox4.Location = new System.Drawing.Point(69, 0);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Padding = new System.Windows.Forms.Padding(5);
            this.pictureBox4.Size = new System.Drawing.Size(69, 70);
            this.pictureBox4.TabIndex = 1;
            this.pictureBox4.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox4, "Venda com nota fiscal.");
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::uniBaterFrenteLoja.Properties.Resources.btnOrcamento;
            this.pictureBox5.Location = new System.Drawing.Point(138, 0);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Padding = new System.Windows.Forms.Padding(3);
            this.pictureBox5.Size = new System.Drawing.Size(69, 70);
            this.pictureBox5.TabIndex = 2;
            this.pictureBox5.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox5, "Fazer orçamento.");
            this.pictureBox5.Click += new System.EventHandler(this.pictureBox5_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::uniBaterFrenteLoja.Properties.Resources.topo;
            this.pictureBox1.InitialImage = global::uniBaterFrenteLoja.Properties.Resources.topo;
            this.pictureBox1.Location = new System.Drawing.Point(10, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(209, 58);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackgroundImage = global::uniBaterFrenteLoja.Properties.Resources.bg;
            this.pictureBox2.Location = new System.Drawing.Point(0, -1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(482, 70);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // toolTip1
            // 
            this.toolTip1.ForeColor = System.Drawing.Color.Black;
            this.toolTip1.IsBalloon = true;
            // 
            // lblModulos
            // 
            this.lblModulos.AutoSize = true;
            this.lblModulos.BackColor = System.Drawing.Color.Red;
            this.lblModulos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModulos.ForeColor = System.Drawing.Color.White;
            this.lblModulos.Location = new System.Drawing.Point(229, 83);
            this.lblModulos.Name = "lblModulos";
            this.lblModulos.Size = new System.Drawing.Size(160, 13);
            this.lblModulos.TabIndex = 51;
            this.lblModulos.Text = "SELECIONE UM MÓDULO:";
            // 
            // login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 304);
            this.Controls.Add(this.flwModulos);
            this.Controls.Add(this.txtSenha);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnEntrar);
            this.Controls.Add(this.cbLojas);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblModulos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "login";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UNIBATER EXTRANET";
            this.Load += new System.EventHandler(this.login_Load);
            this.flwModulos.ResumeLayout(false);
            this.flwModulos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgCupomFiscal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbLojas;
        private System.Windows.Forms.Button btnEntrar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MaskedTextBox txtSenha;
        private System.Windows.Forms.FlowLayoutPanel flwModulos;
        private System.Windows.Forms.PictureBox imgCupomFiscal;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblModulos;
    }
}