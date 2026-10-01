using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using static TT.FrameWork.Identity;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;

namespace TT_Flow.App.Paginas.Mensagem
{
    public partial class Mensagens_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Mensagens";
        string sProcedure = "sp_Manipula_tbl_Flow_Mensagens";


        protected void Page_Load(object sender, EventArgs e)
        {
            
            FUNCOES.ValidaPermissao(Permissao.Mensagens.Consultar, true);
            //RegistraScript("");
            if (!IsPostBack)
            {
                if (!string.IsNullOrWhiteSpace(Request["sMsg"]))
                {
                    MensagemPagina.MostraMensagem_Sucesso(Request["sMsg"]);
                }

                PopularDepartamentos();
                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString());
                    
                }
                else
                {
                    Pesquisar("0");
                }

             
            }
            else
            {
                txtsCorpo.Value = RecuperarCorpoHtml();
            }
        }

        private void PopularDepartamentos()
        {
            cblDepartamentos.Items.Clear();
            lblDepartamentosVazio.Visible = false;

            try
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_DEPARTAMENTOS_MENSAGENS");
                vParametros.Add("@idUsuarioPesquisa", IDENTITY.Variaveis.idUsuario());

                DataSet dsDepartamentos = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsDepartamentos))
                {
                    PopularDepartamentos_DataBind(dsDepartamentos.Tables[0]);
                }
            }
            catch
            {
                DataTable tbDepartamentos = BD.ExecutarDataTable("sp_Select 'Flow_Departamentos'");
                PopularDepartamentos_DataBind(tbDepartamentos);
            }

            if (cblDepartamentos.Items.Count == 0)
            {
                DataTable tbDepartamentos = BD.ExecutarDataTable("sp_Select 'Flow_Departamentos'");
                PopularDepartamentos_DataBind(tbDepartamentos);
            }

            if (cblDepartamentos.Items.Count == 0)
            {
                lblDepartamentosVazio.Visible = true;
            }
        }

        private void PopularDepartamentos_DataBind(DataTable tbDepartamentos)
        {
            if (tbDepartamentos != null && tbDepartamentos.Rows.Count > 0)
            {
                cblDepartamentos.DataSource = tbDepartamentos;
                cblDepartamentos.DataValueField = "idDepartamento";
                cblDepartamentos.DataTextField = "sDscDepartamento";
                cblDepartamentos.DataBind();
            }
        }

        private void PopularUsuarios()
        {
            PopularUsuarios(hddidMensagens.Value == "0");
        }

        private void PopularUsuarios(bool selecionarTodosQuandoNaoHouverSelecao)
        {
            List<string> usuariosSelecionados = RecuperarSelecionados(cblUsuarios).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            cblUsuarios.Items.Clear();
            lblUsuariosVazio.Visible = false;

            if (RecuperarSelecionados(cblDepartamentos) == "")
            {
                lblUsuariosVazio.Text = "Selecione um departamento para listar usuários.";
                lblUsuariosVazio.Visible = true;
                DIV_USUARIOS.Visible = true;
                return;
            }

            DataTable tbUsuarios = CriarTabelaUsuarios();

            foreach (ListItem departamento in cblDepartamentos.Items)
            {
                if (!departamento.Selected)
                {
                    continue;
                }

                DataTable tbDepartamentoUsuarios = BD.ExecutarDataTable("sp_Select 'Usuarios_x_Departamentos'," + departamento.Value);
                AdicionarUsuarios(tbUsuarios, tbDepartamentoUsuarios);
            }

            cblUsuarios.DataSource = tbUsuarios;
            cblUsuarios.DataValueField = "idUsuario";
            cblUsuarios.DataTextField = "sDscUsuarioEmail";
            cblUsuarios.DataBind();

            if (usuariosSelecionados.Count > 0)
            {
                SelecionarItens(cblUsuarios, usuariosSelecionados);
            }
            else if (selecionarTodosQuandoNaoHouverSelecao)
            {
                SelecionarTodos(cblUsuarios);
            }

            DIV_USUARIOS.Visible = cblUsuarios.Items.Count > 0;

            if (cblUsuarios.Items.Count == 0)
            {
                lblUsuariosVazio.Text = "Nenhum usuário encontrado para os departamentos selecionados.";
                lblUsuariosVazio.Visible = true;
                DIV_USUARIOS.Visible = true;
            }
        }

        private DataTable CriarTabelaUsuarios()
        {
            DataTable tbUsuarios = new DataTable();
            tbUsuarios.Columns.Add("idUsuario");
            tbUsuarios.Columns.Add("sDscUsuario");
            tbUsuarios.Columns.Add("sEmail");
            tbUsuarios.Columns.Add("sDscUsuarioEmail");
            return tbUsuarios;
        }

        private void AdicionarUsuarios(DataTable tbDestino, DataTable tbOrigem)
        {
            if (tbOrigem == null)
            {
                return;
            }

            foreach (DataRow row in tbOrigem.Rows)
            {
                string idUsuario = row["idUsuario"].ToString();
                if (tbDestino.Select("idUsuario = '" + idUsuario.Replace("'", "''") + "'").Length > 0)
                {
                    continue;
                }

                string sDscUsuario = row.Table.Columns.Contains("sDscUsuario") ? row["sDscUsuario"].ToString() : idUsuario;
                string sEmail = row.Table.Columns.Contains("sEmail") ? row["sEmail"].ToString() : "";

                DataRow novo = tbDestino.NewRow();
                novo["idUsuario"] = idUsuario;
                novo["sDscUsuario"] = sDscUsuario;
                novo["sEmail"] = sEmail;
                novo["sDscUsuarioEmail"] = string.IsNullOrWhiteSpace(sEmail) ? sDscUsuario : sDscUsuario + " - " + sEmail;
                tbDestino.Rows.Add(novo);
            }
        }



        protected void Pesquisar(string idMensagem)
        {
            string sErro = "";
            try
            {
                LimpaCampos();

                hddidMensagens.Value = "0";
                cmdSalvar.Text = "Enviar";
                cmdSalvar.Visible = false;

                if (idMensagem != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idMensagem", idMensagem);
                    vParametros.Add("@idUsuarioPesquisa", IDENTITY.Variaveis.idUsuario());

                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidMensagens.Value          = RETORNO.DATASET(dsPesquisa, 0, "idMensagem");
                        
                        txtidUsuarioRemetente.Text    = RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioRemetente");
                        txtsAssunto.Text              = RETORNO.DATASET(dsPesquisa, 0, "sAssunto");
                        cbAviso.Checked                = RETORNO.DATASET(dsPesquisa, 0, "sIsAviso") == "S";
                        cbPermiteComentarios.Checked   = RETORNO.DATASET(dsPesquisa, 0, "sPermiteComentario") == "S";
                        txtsTags.Text                  = RETORNO.DATASET(dsPesquisa, 0, "sTags").Replace("|", " ").Trim();
                        txtsCorpo.Value                = HttpUtility.HtmlDecode(RETORNO.DATASET(dsPesquisa, 0, "sCorpo"));

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtInclusao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioRemetente"));

                        hddidMensagens.Value = idMensagem;
                        bool podeEditar = RETORNO.DATASET(dsPesquisa, 0, "idUsuarioRemetente") == IDENTITY.Variaveis.idUsuario();
                        hddPodeEditar.Value = podeEditar ? "S" : "N";
                        hddEmEdicao.Value = "N";

                        lblTituloPagina.Text = string.Format("Mensagem - {0}", txtsAssunto.Text.ToString().PadLeft('0'));
                        BreadCrumb.TitulodaPagina = string.Format("Mensagem - n.º {0}", idMensagem.ToString().PadLeft(4, '0'));

                        if (dsPesquisa.Tables.Count > 2)
                        {
                            SelecionarItens(cblDepartamentos, dsPesquisa.Tables[2], "idDepartamento");
                        }
                        else
                        {
                            SelecionarItens(cblDepartamentos, RETORNO.DATASET(dsPesquisa, 0, "idDepartamentoDestino"));
                        }

                        if (RecuperarSelecionados(cblDepartamentos) == "" && dsPesquisa.Tables.Count > 1)
                        {
                            SelecionarDepartamentosPorUsuariosSalvos(dsPesquisa.Tables[1]);
                        }

                        PopularUsuarios(false);

                        if (dsPesquisa.Tables.Count > 1)
                        {
                            AdicionarUsuariosSalvos(dsPesquisa.Tables[1]);
                            SelecionarItens(cblUsuarios, dsPesquisa.Tables[1], "idUsuarioDestino");
                        }
 
                        PreencherDestinatarios(dsPesquisa);
                        PreencherComentarios(dsPesquisa);
                        PreencherHistorico(dsPesquisa);
                        AplicarModoEdicao(false);

                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Mensagens.Enviar, true);
                    BreadCrumb.TitulodaPagina = "Nova Mensagem";
                    cmdSalvar.Visible = true;

                    txtidUsuarioRemetente.Text = IDENTITY.Variaveis.sUsuarioLogado();
                    hddPodeEditar.Value = "S";
                    hddEmEdicao.Value = "S";
                    AplicarModoEdicao(true);
                    PopularUsuarios();

                }

                txtsAssunto.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }


        void LimpaCampos()
        {
            LimparSelecao(cblDepartamentos);
            cblUsuarios.Items.Clear();
            txtidUsuarioRemetente.Text = "";
            txtsAssunto.Text = "";
            txtsTags.Text = "";
            txtsComentario.Text = "";
            txtsCorpo.Value = "";
            cbAviso.Checked = false;
            cbPermiteComentarios.Checked = false;

            cmdSalvar.Text = "Salvar";
            cmdEditar.Visible = false;
            hddPodeEditar.Value = "N";
            hddEmEdicao.Value = "N";
            lblTituloPagina.Text = sTituloPagina;
            DIV_Destinatario_GRUPO.Visible = true;
            DIV_USUARIOS.Visible = false;
            PainelAtualizacao.Visible = false;
            pnlComentarios.Visible = false;
            pnlHistorico.Visible = false;
            divAvisoDashboard.Style["visibility"] = "hidden";
        }

        private string RecuperarSelecionados(CheckBoxList lista)
        {
            return string.Join(",", lista.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value));
        }

        private void LimparSelecao(CheckBoxList lista)
        {
            foreach (ListItem item in lista.Items)
            {
                item.Selected = false;
            }
        }

        private void SelecionarTodos(CheckBoxList lista)
        {
            foreach (ListItem item in lista.Items)
            {
                item.Selected = true;
            }
        }

        private void SelecionarItens(CheckBoxList lista, string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return;
            }

            SelecionarItens(lista, valor.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()).ToList());
        }

        private void SelecionarItens(CheckBoxList lista, DataTable tabela, string nomeColuna)
        {
            if (tabela == null || !tabela.Columns.Contains(nomeColuna))
            {
                return;
            }

            List<string> valores = new List<string>();
            foreach (DataRow row in tabela.Rows)
            {
                valores.Add(row[nomeColuna].ToString());
            }

            SelecionarItens(lista, valores);
        }

        private void SelecionarItens(CheckBoxList lista, List<string> valores)
        {
            foreach (ListItem item in lista.Items)
            {
                item.Selected = valores.Contains(item.Value);
            }
        }

        private void AdicionarUsuariosSalvos(DataTable tabela)
        {
            if (tabela == null || !tabela.Columns.Contains("idUsuarioDestino"))
            {
                return;
            }

            foreach (DataRow row in tabela.Rows)
            {
                string idUsuario = row["idUsuarioDestino"].ToString();
                if (string.IsNullOrWhiteSpace(idUsuario) || cblUsuarios.Items.FindByValue(idUsuario) != null)
                {
                    continue;
                }

                string nome = tabela.Columns.Contains("sDscUsuario") ? row["sDscUsuario"].ToString() : idUsuario;
                string email = tabela.Columns.Contains("sEmail") ? row["sEmail"].ToString() : "";
                string texto = string.IsNullOrWhiteSpace(email) ? nome : nome + " - " + email;

                cblUsuarios.Items.Add(new ListItem(texto, idUsuario));
            }

            DIV_USUARIOS.Visible = cblUsuarios.Items.Count > 0;
            lblUsuariosVazio.Visible = cblUsuarios.Items.Count == 0;
        }

        private void SelecionarDepartamentosPorUsuariosSalvos(DataTable tabela)
        {
            if (tabela == null || !tabela.Columns.Contains("idUsuarioDestino"))
            {
                return;
            }

            List<string> idsUsuarios = tabela.Rows.Cast<DataRow>()
                .Select(row => row["idUsuarioDestino"].ToString())
                .Where(idUsuario => !string.IsNullOrWhiteSpace(idUsuario))
                .Distinct()
                .ToList();

            if (idsUsuarios.Count == 0)
            {
                return;
            }

            foreach (ListItem departamento in cblDepartamentos.Items)
            {
                DataTable tbUsuariosDepartamento = BD.ExecutarDataTable("sp_Select 'Usuarios_x_Departamentos'," + departamento.Value);
                if (tbUsuariosDepartamento == null || !tbUsuariosDepartamento.Columns.Contains("idUsuario"))
                {
                    continue;
                }

                bool possuiUsuarioSalvo = tbUsuariosDepartamento.Rows.Cast<DataRow>()
                    .Any(row => idsUsuarios.Contains(row["idUsuario"].ToString()));

                if (possuiUsuarioSalvo)
                {
                    departamento.Selected = true;
                }
            }
        }

        private string RecuperarPrimeiroSelecionado(CheckBoxList lista)
        {
            ListItem item = lista.Items.Cast<ListItem>().FirstOrDefault(i => i.Selected);
            return item == null ? "0" : item.Value;
        }

        private string NormalizarTags(string tags)
        {
            List<string> tagsNormalizadas = RecuperarTagsNormalizadas(tags);
            return tagsNormalizadas.Count == 0 ? "" : "|" + string.Join("|", tagsNormalizadas) + "|";
        }

        private List<string> RecuperarTagsNormalizadas(string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                return new List<string>();
            }

            return tags.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToList();
        }

        private string RecuperarCorpoHtml()
        {
            string corpoHtml = HttpUtility.UrlDecode(hddsCorpoHtml.Value ?? "");
            if (string.IsNullOrWhiteSpace(corpoHtml))
            {
                corpoHtml = txtsCorpo.Value;
            }

            return corpoHtml ?? "";
        }

        private string RecuperarTextoCorpo(string corpoHtml)
        {
            if (string.IsNullOrWhiteSpace(corpoHtml))
            {
                return "";
            }

            string texto = Regex.Replace(corpoHtml, "<[^>]*>", " ");
            texto = HttpUtility.HtmlDecode(texto).Replace("&nbsp;", " ");
            texto = Regex.Replace(texto, @"\s+", " ").Trim();
            return texto;
        }

        private void AplicarModoEdicao(bool podeEditar)
        {
            txtidUsuarioRemetente.ReadOnly = true;
            txtsAssunto.ReadOnly = !podeEditar;
            txtsTags.ReadOnly = !podeEditar;
            cbAviso.Enabled = podeEditar;
            cbPermiteComentarios.Enabled = podeEditar;
            cblDepartamentos.Enabled = podeEditar;
            cblUsuarios.Enabled = podeEditar;
            cmdSalvar.Visible = podeEditar;
            cmdEditar.Visible = hddidMensagens.Value != "0" && hddPodeEditar.Value == "S" && !podeEditar;

            string css = txtsCorpo.Attributes["class"] ?? "";
            css = css.Replace("naoEdita", "").Trim();
            txtsCorpo.Attributes["class"] = podeEditar ? css : (css + " naoEdita").Trim();
            txtsCorpo.Attributes.Remove("readonly");
            if (!podeEditar)
            {
                txtsCorpo.Attributes.Add("readonly", "readonly");
            }
        }


        private bool ValidarDados()
        {
            if (hddidMensagens.Value != "0" && hddPodeEditar.Value != "S")
            {
                MensagemPagina.MostraMensagem_Erro("A mensagem só pode ser editada pelo próprio remetente.");
                return false;
            }
            if (hddidMensagens.Value != "0" && hddEmEdicao.Value != "S")
            {
                MensagemPagina.MostraMensagem_Erro("Clique em Editar antes de salvar alterações nesta mensagem.");
                return false;
            }
            if (txtsAssunto.Text.Trim().Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Informe o assunto da mensagem.");
                return false;
            }

            int totalTagsInformadas = txtsTags.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
            if (totalTagsInformadas > 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe no máximo 3 tags por mensagem.");
                return false;
            }

            string corpoHtml = RecuperarCorpoHtml();
            string corpoTexto = RecuperarTextoCorpo(corpoHtml);
            if (string.IsNullOrWhiteSpace(corpoTexto))
            {
                MensagemPagina.MostraMensagem_Erro("Informe o corpo da mensagem. O editor enviou conteúdo vazio.");
                return false;
            }

            if (corpoTexto.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("O corpo da mensagem está muito curto. Texto identificado: \"" + corpoTexto + "\".");
                return false;
            }

            if (RecuperarSelecionados(cblUsuarios) == "")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione ao menos um usuário destinatário. Departamentos selecionados: " + cblDepartamentos.Items.Cast<ListItem>().Count(item => item.Selected) + ".");
                return false;
            }

            return true;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (!FUNCOES.ValidaPermissao(Permissao.Mensagens.Enviar, false))
            {
                MensagemPagina.MostraMensagem_Erro("Usuário sem permissão para enviar mensagens.");
                return;
            }

            if (ValidarDados())
            {
                try
                {
                    string[] vidMensagem = hddidMensagens.Value.Split(',');
                    string idMensagem = vidMensagem[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao",                         "SALVAR");
                    vParametros.Add("@idMensagem",                      idMensagem);
                    vParametros.Add("@idUsuarioRemetente",              Variaveis.idUsuario());         
                    vParametros.Add("@idDepartamentoDestino",           "0");
                    vParametros.Add("@sDepartamentosDestino",           RecuperarSelecionados(cblDepartamentos));
                    vParametros.Add("@sUsuariosDestino",                RecuperarSelecionados(cblUsuarios));
                    vParametros.Add("@idTipoObjeto",                    "0");
                    string sCorpoHtml = RecuperarCorpoHtml();
                    vParametros.Add("@sCorpo",                          sCorpoHtml);
                    vParametros.Add("@sAssunto",                        txtsAssunto.Text);
                    vParametros.Add("@sIsAviso",                        cbAviso.Checked ? "S" : "N");
                    vParametros.Add("@sPermiteComentario",              cbPermiteComentarios.Checked ? "S" : "N");
                    vParametros.Add("@sTags",                           NormalizarTags(txtsTags.Text));
                    vParametros.Add("@idUsuarioAtualizacao",             Variaveis.idUsuario());
                    vParametros.Add("@idUsuarioDestino",                RecuperarPrimeiroSelecionado(cblUsuarios));

                    dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        string idMensagemSalva = dsSalvar.Tables[0].Rows[0]["idMensagem"].ToString();
                        Response.Redirect(
                            "Mensagens_Detalhe.aspx?id=" + idMensagemSalva
                            + "&sMsg=" + HttpUtility.UrlEncode("Mensagem gravada com sucesso"),
                            false);
                    }
                    else
                    {
                        throw new Exception("A procedure não confirmou o salvamento. Retorno BD: " + sErro.ToString()
                            + " | idMensagem: " + idMensagem
                            + " | departamentos: " + RecuperarSelecionados(cblDepartamentos)
                            + " | usuários: " + RecuperarSelecionados(cblUsuarios)
                            + " | tamanhoCorpoHtml: " + sCorpoHtml.Length);
                    }
                }
                catch (Exception ex)
                {
                    txtsCorpo.Value = RecuperarCorpoHtml();
                    MensagemPagina.MostraMensagem_Erro("Erro ao salvar mensagem. Detalhes: " + ex.Message);
                }

            }
            else
            {
                txtsCorpo.Value = RecuperarCorpoHtml();
            }

        }

        protected void cmdEditar_Click(object sender, EventArgs e)
        {
            if (hddidMensagens.Value == "0" || hddPodeEditar.Value != "S")
            {
                MensagemPagina.MostraMensagem_Erro("A mensagem só pode ser editada pelo próprio remetente.");
                return;
            }

            hddEmEdicao.Value = "S";
            txtsCorpo.Value = RecuperarCorpoHtml();
            AplicarModoEdicao(true);
        }
        protected void cblDepartamentos_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularUsuarios();
        }


        protected void PreencherDestinatarios(DataSet  dsMensagem)
        {

            if (dsMensagem.Tables.Count > 1)
            {
                dtgDestinatarios.DataSource = dsMensagem.Tables[1];
                dtgDestinatarios.DataBind();
            }

        }

        protected void PreencherComentarios(DataSet dsMensagem)
        {
            pnlComentarios.Visible = hddidMensagens.Value != "0" && cbPermiteComentarios.Checked;
            cmdSalvarComentario.Visible = hddidMensagens.Value != "0" && cbPermiteComentarios.Checked;

            if (dsMensagem.Tables.Count > 3)
            {
                rptComentarios.DataSource = dsMensagem.Tables[3];
                rptComentarios.DataBind();
            }
        }

        protected void PreencherHistorico(DataSet dsMensagem)
        {
            pnlHistorico.Visible = hddidMensagens.Value != "0";

            if (dsMensagem.Tables.Count > 4)
            {
                dtgHistorico.DataSource = dsMensagem.Tables[4];
                dtgHistorico.DataBind();
            }

        }

        protected void cmdSalvarComentario_Click(object sender, EventArgs e)
        {
            if (hddidMensagens.Value == "0" || !cbPermiteComentarios.Checked)
            {
                MensagemPagina.MostraMensagem_Erro("Esta mensagem não permite comentários.");
                return;
            }

            if (txtsComentario.Text.Trim().Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Comentário inválido.");
                return;
            }

            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "SALVAR_COMENTARIO");
            vParametros.Add("@idMensagem", hddidMensagens.Value);
            vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
            vParametros.Add("@sDscComentario", txtsComentario.Text.Trim());

            DataSet dsComentario = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsComentario))
            {
                txtsComentario.Text = "";
                txtsCorpo.Value = RecuperarCorpoHtml();
                Pesquisar(hddidMensagens.Value);
                MensagemPagina.MostraMensagem_Sucesso("Comentário gravado com sucesso.");
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao gravar comentário.");
            }

        }


    }
}