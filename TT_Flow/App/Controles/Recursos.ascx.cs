using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BD = TT.FrameWork.BD;
using TT.FrameWork;

namespace TT_Flow.App.Controles
{
    public partial class Recursos : UserControl
    {
        #region | Construtores

        public string sRecursos { get { return _sRecursos; } set { _sRecursos = value; } }
        public bool ReadOnly = false;
        public bool Enable = true;
        private string _sRecursos = "";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                tv.Attributes.Add("onclick", "OnTreeClick(event);");

            if (ReadOnly)
                tv.Enabled = !ReadOnly;

            if (Enable)
                tv.Enabled = Enable;
        }

        public void ConsultarPermissao_Perfil(string idPerfil) => Consultar("0", idPerfil);

        public void ConsultarPermissao_Usuario(string idUsuario) => Consultar(idUsuario, "0");

        public void ConsultarPermissao(string idPerfil)
        {
            tv.Nodes.Clear();
            Consultar("0", idPerfil);
            tv.ExpandAll();
        }

        public string RecuperarPermissao()
        {
            string sRetorno = "";

            if (tv.CheckedNodes.Count > 0)
            {
                foreach (TreeNode node in tv.CheckedNodes)
                {
                    sRetorno += "|" + node.Value.ToString();
                }

                sRetorno += "|";
            }

            return sRetorno;
        }

        void Consultar(string idUsuario, string idPerfil)
        {
            if (tv.Nodes.Count == 0)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTA_DETALHE" },
                    { "@idUsuario", idUsuario },
                    { "@idPerfil", idPerfil },
                    { "@sRecursos", sRecursos }
                };
                DataSet dsPermissao = BD.ExecutarDataSet("sp_Manipula_tbl_Recursos", vParametros);

                if (BD.ValidarDataSet(dsPermissao, out string sErro))
                    CriarNo(null, dsPermissao);
            }

            tv.Enabled = Enable;
        }

        void CriarNo(TreeNode NoPai, DataSet ds)
        {
            string idRecursoPai_Original = "0";

            if (NoPai != null)
                idRecursoPai_Original = NoPai.Value;

            DataRow[] dr = ds.Tables[0].Select(string.Format("idrecursoPai={0}", idRecursoPai_Original), "nOrdem, idRecurso, sDscRecurso");

            if (dr.Count() > 0)
            {
                foreach (DataRow row in dr)
                {
                    string idRecurso = row["idRecurso"].ToString();
                    string idRecursoPai_ = row["idRecursoPai"].ToString();
                    string sDscRecurso = row["sDscRecurso"].ToString();
                    string sLiberado = row["sLiberado"].ToString();

                    TreeNode node = new TreeNode(sDscRecurso, idRecurso);

                    if (sLiberado == "S")
                        node.Checked = true;

                    if (NoPai != null)
                    {
                        if (NoPai.Depth > 0)
                            node.Expanded = false;

                        NoPai.ChildNodes.Add(node);
                    }
                    else
                        tv.Nodes.Add(node);

                    CriarNo(node, ds);
                }
            }
        }

        protected void cmdTodos_Click(object sender, EventArgs e)
        {
            if ((sender as LinkButton).ID.Contains("Abrir"))
            {
                tv.ExpandAll();
                cmdAbrirTodos.Attributes["class"] += " invisivel";
                cmdFecharTodos.Attributes["class"] = cmdFecharTodos.Attributes["class"].Replace("invisivel", "");
            }
            else
            {
                tv.CollapseAll();
                cmdAbrirTodos.Attributes["class"] = cmdAbrirTodos.Attributes["class"].Replace("invisivel", "");
                cmdFecharTodos.Attributes["class"] += " invisivel";
            }
        }
    }
}