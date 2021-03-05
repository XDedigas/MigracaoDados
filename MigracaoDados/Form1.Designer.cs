namespace MigracaoDados
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbResultado = new System.Windows.Forms.TextBox();
            this.tbConexao = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbPlanilha = new System.Windows.Forms.ComboBox();
            this.btIniciarMigracao = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbResultado
            // 
            this.tbResultado.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tbResultado.Location = new System.Drawing.Point(12, 61);
            this.tbResultado.Multiline = true;
            this.tbResultado.Name = "tbResultado";
            this.tbResultado.ReadOnly = true;
            this.tbResultado.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResultado.Size = new System.Drawing.Size(497, 377);
            this.tbResultado.TabIndex = 3;
            // 
            // tbConexao
            // 
            this.tbConexao.Location = new System.Drawing.Point(67, 8);
            this.tbConexao.Name = "tbConexao";
            this.tbConexao.Size = new System.Drawing.Size(258, 20);
            this.tbConexao.TabIndex = 9;
            this.tbConexao.Text = "data source=DESKTOP-846A5BI;initial catalog = gerenciadorpalpites; persist securi" +
    "ty info = True;Integrated Security = SSPI;";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Conexão:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Planilha:";
            // 
            // cbPlanilha
            // 
            this.cbPlanilha.FormattingEnabled = true;
            this.cbPlanilha.Location = new System.Drawing.Point(67, 34);
            this.cbPlanilha.Name = "cbPlanilha";
            this.cbPlanilha.Size = new System.Drawing.Size(258, 21);
            this.cbPlanilha.TabIndex = 12;
            // 
            // btIniciarMigracao
            // 
            this.btIniciarMigracao.Location = new System.Drawing.Point(331, 8);
            this.btIniciarMigracao.Name = "btIniciarMigracao";
            this.btIniciarMigracao.Size = new System.Drawing.Size(178, 47);
            this.btIniciarMigracao.TabIndex = 1;
            this.btIniciarMigracao.Text = "Iniciar Migração";
            this.btIniciarMigracao.UseVisualStyleBackColor = true;
            this.btIniciarMigracao.Click += new System.EventHandler(this.btIniciarMigracao_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 450);
            this.Controls.Add(this.cbPlanilha);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbConexao);
            this.Controls.Add(this.tbResultado);
            this.Controls.Add(this.btIniciarMigracao);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbResultado;
        private System.Windows.Forms.TextBox tbConexao;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbPlanilha;
        private System.Windows.Forms.Button btIniciarMigracao;
    }
}

