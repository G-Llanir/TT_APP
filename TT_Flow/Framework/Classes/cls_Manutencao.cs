using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using static TT.FrameWork.Identity;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_Departamentos_Usuarios
    {

        #region | Construtor 
        public cls_Departamentos_Usuarios()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Departamentos_Usuarios
        (
              int idRegistro
            , int idDepartamento
            , string sDscDepartamento
            , int idUsuario
            , string sDscUsuario
            , string sNotificacaoEmail
            , string sGestorDepartamento
        )
        {
            _idRegistro = idRegistro;
            _idDepartamento = idDepartamento;
            _sDscDepartamento = sDscDepartamento;
            _idUsuario = idUsuario;
            _sDscUsuario = sDscUsuario;
            _sNotificacaoEmail = sNotificacaoEmail;
            _sGestorDepartamento = sGestorDepartamento;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistro;
        private int _idDepartamento;
        private string _sDscDepartamento;
        private int _idUsuario;
        private string _sDscUsuario;
        private string _sNotificacaoEmail;
        private string _sGestorDepartamento;
        #endregion

        #region | Propriedades

        public int idRegistro
        {
            get { return _idRegistro; }
            set { _idRegistro = value; }
        }

        public int idDepartamento
        {
            get { return _idDepartamento; }
            set { _idDepartamento = value; }
        }

        public string sDscDepartamento
        {
            get { return _sDscDepartamento; }
            set { _sDscDepartamento = value; }
        }
        public int idUsuario
        {
            get { return _idUsuario; }
            set { _idUsuario = value; }
        }

        public string sDscUsuario
        {
            get { return _sDscUsuario; }
            set { _sDscUsuario = value; }
        }


        public string sNotificacaoEmail
        {
            get { return _sNotificacaoEmail; }
            set { _sNotificacaoEmail = value; }
        }

        public string sGestorDepartamento
        {
            get { return _sGestorDepartamento; }
            set { _sGestorDepartamento = value; }
        }

        #endregion


    }

    [Serializable]
    public class cls_Usuarios_Empresa
    {
        #region | Construtor

        public cls_Usuarios_Empresa()
        {

        }
        public cls_Usuarios_Empresa
        (
              int idEmpresa
            , string sDscEmpresa
            , int idLinha
            , string sFuncao
            , int idRegistro
            , int idUsuario
        )
        {
            _idEmpresa = idEmpresa;
            _sDscEmpresa = sDscEmpresa;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idRegistro = idRegistro;
            _idUsuario = idUsuario;
        }

        #endregion

        #region | Membros Privados

        private int _idEmpresa;
        private string _sDscEmpresa;
        private int _idLinha;
        private string _sFuncao;
        private int _idRegistro;
        private int _idUsuario;
        #endregion

        #region | Propriedades

        public int idEmpresa
        {
            get { return _idEmpresa; }
            set { _idEmpresa = value; }
        }

        public string sDscEmpresa
        {
            get { return _sDscEmpresa; }
            set { _sDscEmpresa = value; }
        }

        public int idLinha
        {
            get { return _idLinha; }
            set { _idLinha = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        public int idRegistro
        {
            get { return _idRegistro; }
            set { _idRegistro = value; }
        }

        public int idUsuario
        {
            get { return _idUsuario; }
            set { _idUsuario = value; }
        }

        #endregion
    }

    [Serializable]
    public class cls_Fluxo_x_Departamentos
    {

        #region | Construtor 
        public cls_Fluxo_x_Departamentos()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Fluxo_x_Departamentos
        (
              int idRegistro
            , int idFluxo
            , int idDepartamento
            , string sDscDepartamento
            , int nOrdem
            , int idStatus
            , string sDscStatus
            , int nTempo
            , string sTipoTempo
            , int idStatusKanban
        )
        {
            _idRegistro = idRegistro;
            _idFluxo = idFluxo;
            _idDepartamento = idDepartamento;
            _sDscDepartamento = sDscDepartamento;
            _nOrdem = nOrdem;
            _idStatus = idStatus;
            _sDscStatus = sDscStatus;
            _nTempo = nTempo;
            _sTipoTempo = sTipoTempo;
            _idStatusKanban = idStatusKanban;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistro;
        private int _idFluxo;
        private int _idDepartamento;
        string _sDscDepartamento;
        private int _nOrdem;
        private int _idStatus;
        private string _sDscStatus;
        private int _nTempo;
        private string _sTipoTempo;
        private int _idStatusKanban;
        #endregion

        #region | Propriedades

        public int idRegistro
        {
            get { return _idRegistro; }
            set { _idRegistro = value; }
        }

        public int idFluxo
        {
            get { return _idFluxo; }
            set { _idFluxo = value; }
        }
        public int idDepartamento
        {
            get { return _idDepartamento; }
            set { _idDepartamento = value; }
        }

        public string sDscDepartamento
        {
            get { return _sDscDepartamento; }
            set { _sDscDepartamento = value; }
        }

        public int nOrdem
        {
            get { return _nOrdem; }
            set { _nOrdem = value; }
        }

        public int idStatus
        {
            get { return _idStatus; }
            set { _idStatus = value; }
        }

        public string sDscStatus
        {
            get { return _sDscStatus; }
            set { _sDscStatus = value; }
        }

        public int nTempo
        {
            get { return _nTempo; }
            set { _nTempo = value; }
        }

        public string sTipoTempo
        {
            get { return _sTipoTempo; }
            set { _sTipoTempo = value; }
        }

        public int idStatusKanban
        {
            get { return _idStatusKanban; }
            set { _idStatusKanban = value; }
        }

        #endregion


    }

    [Serializable]
    public class cls_Fluxo_x_Tarefas
    {

        #region | Construtor 
        public cls_Fluxo_x_Tarefas()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Fluxo_x_Tarefas
        (
              int idRegistro
            , int idFluxo
            , int idDepartamento
            , string sDscDepartamento
            , int nOrdem
            , int idTarefa
            , string sDscTarefa
            , int nTempo
            , string sTipoTempo
            , int nDegrau
            , string sObrigatorioConclusao
        )
        {
            _idRegistro = idRegistro;
            _idFluxo = idFluxo;
            _idDepartamento = idDepartamento;
            _sDscDepartamento = sDscDepartamento;
            _nOrdem = nOrdem;
            _idTarefa = idTarefa;
            _sDscTarefa = sDscTarefa;
            _nTempo = nTempo;
            _sTipoTempo = sTipoTempo;
            _nDegrau = nDegrau;
            _sObrigatorioConclusao = sObrigatorioConclusao;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistro;
        private int _idFluxo;
        private int _idDepartamento;
        string _sDscDepartamento;
        private int _nOrdem;
        private int _idTarefa;
        private string _sDscTarefa;
        private int _nTempo;
        private string _sTipoTempo;
        private int _nDegrau;
        private string _sObrigatorioConclusao;

        #endregion

        #region | Propriedades

        public int idRegistro
        {
            get { return _idRegistro; }
            set { _idRegistro = value; }
        }

        public int idFluxo
        {
            get { return _idFluxo; }
            set { _idFluxo = value; }
        }
        public int idDepartamento
        {
            get { return _idDepartamento; }
            set { _idDepartamento = value; }
        }

        public string sDscDepartamento
        {
            get { return _sDscDepartamento; }
            set { _sDscDepartamento = value; }
        }

        public int nOrdem
        {
            get { return _nOrdem; }
            set { _nOrdem = value; }
        }

        public int idTarefa
        {
            get { return _idTarefa; }
            set { _idTarefa = value; }
        }

        public string sDscTarefa
        {
            get { return _sDscTarefa; }
            set { _sDscTarefa = value; }
        }

        public int nTempo
        {
            get { return _nTempo; }
            set { _nTempo = value; }
        }

        public string sTipoTempo
        {
            get { return _sTipoTempo; }
            set { _sTipoTempo = value; }
        }

        public int nDegrau
        {
            get { return _nDegrau; }
            set { _nDegrau = value; }
        }

        public string sObrigatorioConclusao
        {
            get { return _sObrigatorioConclusao; }
            set { _sObrigatorioConclusao = value; }
        }




        #endregion

    }

    [Serializable]
    public class cls_Fluxo_x_Recursos
    {

        #region | Construtor 
        public cls_Fluxo_x_Recursos()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Fluxo_x_Recursos
        (
              int idRegistro
            , int idFluxo
            , int idRecurso
            , string sDscRecurso
            , int idTipoRecurso
            , string sDscTipoRecurso
            , int nOrdem
            , string sUnidade
            , double nQuantidade
        )
        {
            _idRegistro = idRegistro;
            _idFluxo = idFluxo;
            _idRecurso = idRecurso;
            _sDscRecurso = sDscRecurso;
            _idTipoRecurso = idTipoRecurso;
            _sDscTipoRecurso = sDscTipoRecurso;
            _nOrdem = nOrdem;
            _sUnidade = sUnidade;
            _nQuantidade = nQuantidade;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistro;
        private int _idFluxo;
        private int _idRecurso;
        private string _sDscRecurso;
        private int _idTipoRecurso;
        private string _sDscTipoRecurso;
        private int _nOrdem;
        private string _sUnidade;
        private double _nQuantidade;

        #endregion

        #region | Propriedades

        public int idRegistro
        {
            get { return _idRegistro; }
            set { _idRegistro = value; }
        }
        public int idFluxo
        {
            get { return _idFluxo; }
            set { _idFluxo = value; }
        }
        public int idRecurso
        {
            get { return _idRecurso; }
            set { _idRecurso = value; }
        }
        public string sDscRecurso
        {
            get { return _sDscRecurso; }
            set { _sDscRecurso = value; }
        }
        public int idTipoRecurso
        {
            get { return _idTipoRecurso; }
            set { _idTipoRecurso = value; }
        }
        public string sDscTipoRecurso
        {
            get { return _sDscTipoRecurso; }
            set { _sDscTipoRecurso = value; }
        }
        public int nOrdem
        {
            get { return _nOrdem; }
            set { _nOrdem = value; }
        }
        public string sUnidade
        {
            get { return _sUnidade; }
            set { _sUnidade = value; }
        }
        public double nQuantidade
        {
            get { return _nQuantidade; }
            set { _nQuantidade = value; }
        }

        #endregion

    }

    [Serializable]
    public class cls_Tarefas
    {

        #region | Construtor 
        public cls_Tarefas()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Tarefas
        (
             int idDepartamento
            , string sDscDepartamento
            , int idTarefa
            , string sDscTarefa
            , int nTempo
            , string sTipoTempo
            , string sObrigatorioConclusao
        )
        {
            _idDepartamento = idDepartamento;
            _sDscDepartamento = sDscDepartamento;
            _idTarefa = idTarefa;
            _sDscTarefa = sDscTarefa;
            _nTempo = nTempo;
            _sTipoTempo = sTipoTempo;
            _sObrigatorioConclusao = sObrigatorioConclusao;
        }

        #endregion

        #region | Membros Privados 

        private int _idDepartamento;
        string _sDscDepartamento;
        private int _idTarefa;
        private string _sDscTarefa;
        private int _nTempo;
        private string _sTipoTempo;
        private string _sObrigatorioConclusao;

        #endregion

        #region | Propriedades

        public int idDepartamento
        {
            get { return _idDepartamento; }
            set { _idDepartamento = value; }
        }

        public string sDscDepartamento
        {
            get { return _sDscDepartamento; }
            set { _sDscDepartamento = value; }
        }

        public int idTarefa
        {
            get { return _idTarefa; }
            set { _idTarefa = value; }
        }

        public string sDscTarefa
        {
            get { return _sDscTarefa; }
            set { _sDscTarefa = value; }
        }

        public int nTempo
        {
            get { return _nTempo; }
            set { _nTempo = value; }
        }

        public string sTipoTempo
        {
            get { return _sTipoTempo; }
            set { _sTipoTempo = value; }
        }

        public string sObrigatorioConclusao
        {
            get { return _sObrigatorioConclusao; }
            set { _sObrigatorioConclusao = value; }
        }



        #endregion

    }

    [Serializable]
    public class cls_Recursos
    {

        #region | Construtor 
        public cls_Recursos()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Recursos
        (
             int idTipoRecurso
            , string sDscTipoRecurso
            , int idRecurso
            , string sDscRecurso
            , int nOrdem
            , string sUnidade
            , double nQuantidade
        )
        {
            _idTipoRecurso = idTipoRecurso;
            _sDscTipoRecurso = sDscTipoRecurso;
            _idRecurso = idRecurso;
            _sDscRecurso = sDscRecurso;
            _nOrdem = nOrdem;
            _sUnidade = sUnidade;
            _nQuantidade = nQuantidade;
        }

        #endregion

        #region | Membros Privados 

        private int _idTipoRecurso;
        string _sDscTipoRecurso;
        private int _idRecurso;
        private string _sDscRecurso;
        private int _nOrdem;
        private string _sUnidade;
        private double _nQuantidade;
        #endregion

        #region | Propriedades

        public int idTipoRecurso
        {
            get { return _idTipoRecurso; }
            set { _idTipoRecurso = value; }
        }
        public string sDscTipoRecurso
        {
            get { return _sDscTipoRecurso; }
            set { _sDscTipoRecurso = value; }
        }
        public int idRecurso
        {
            get { return _idRecurso; }
            set { _idRecurso = value; }
        }
        public string sDscRecurso
        {
            get { return _sDscRecurso; }
            set { _sDscRecurso = value; }
        }
        public int nOrdem
        {
            get { return _nOrdem; }
            set { _nOrdem = value; }
        }
        public string sUnidade
        {
            get { return _sUnidade; }
            set { _sUnidade = value; }
        }
        public double nQuantidade
        {
            get { return _nQuantidade; }
            set { _nQuantidade = value; }
        }

        #endregion

    }

    [Serializable]
    public class cls_Clientes_Contatos
    {

        #region | Construtor 
        public cls_Clientes_Contatos()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Clientes_Contatos
        (
              int idContato
            , int idCliente
            , string sTipoContato
            , string sNome
            , string sTelefone
            , string sEmail
            , string sRecebeEmail
            , bool bExcluir
        )
        {
            _idContato = idContato;
            _idCliente = idCliente;
            _sTipoContato = sTipoContato;
            _sNome = sNome;
            _sTelefone = sTelefone;
            _sEmail = sEmail;
            _sRecebeEmail = sRecebeEmail;
            _bExcluir = bExcluir;
        }

        #endregion

        #region | Membros Privados 

        private int _idContato;
        private int _idCliente;
        private string _sTipoContato;
        private string _sNome;
        private string _sTelefone;
        private string _sEmail;
        private string _sRecebeEmail;
        private bool _bExcluir;

        #endregion

        #region | Propriedades

        public int idContato
        {
            get { return _idContato; }
            set { _idContato = value; }
        }

        public int idCliente
        {
            get { return _idCliente; }
            set { _idCliente = value; }
        }

        public string sTipoContato
        {
            get { return _sTipoContato; }
            set { _sTipoContato = value; }
        }

        public string sNome
        {
            get { return _sNome; }
            set { _sNome = value; }
        }

        public string sTelefone
        {
            get { return _sTelefone; }
            set { _sTelefone = value; }
        }

        public string sEmail
        {
            get { return _sEmail; }
            set { _sEmail = value; }
        }

        public string sRecebeEmail
        {
            get { return _sRecebeEmail; }
            set { _sRecebeEmail = value; }
        }

        public bool bExcluir
        {
            get { return _bExcluir; }
            set { _bExcluir = value; }
        }
        #endregion

    }
    [Serializable]
    public class cls_Clientes_Enderecos
    {

        #region | Construtor 
        public cls_Clientes_Enderecos()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Clientes_Enderecos
        (
              int idEndereco
            , int idCliente
            , int idTipoEndereco
            , string sDscTipoEndereco
            , string sCEP
            , string sLogradouro
            , string sNumero
            , string sComplemento
            , string sBairro
            , string sCidade
            , string sEstado
            , string sPais
            , string sEnderecoCompleto
            , string sEnderecoEstrangeiro
            , bool bExcluir
        )
        {
            _idEndereco = idEndereco;
            _idCliente = idCliente;
            _idTipoEndereco = idTipoEndereco;
            _sDscTipoEndereco = sDscTipoEndereco;
            _sCEP = sCEP;
            _sLogradouro = sLogradouro;
            _sNumero = sNumero;
            _sComplemento = sComplemento;
            _sBairro = sBairro;
            _sCidade = sCidade;
            _sEstado = sEstado;
            _sPais = sPais;
            _sEnderecoCompleto = sEnderecoCompleto;
            _sEnderecoEstrangeiro = sEnderecoEstrangeiro;
            _bExcluir = bExcluir;
        }

        #endregion

        #region | Membros Privados 

        private int _idEndereco;
        private int _idCliente;
        private int _idTipoEndereco;
        private string _sDscTipoEndereco;
        private string _sCEP;
        private string _sLogradouro;
        private string _sNumero;
        private string _sComplemento;
        private string _sBairro;
        private string _sCidade;
        private string _sEstado;
        private string _sPais;
        private string _sEnderecoCompleto;
        private string _sEnderecoEstrangeiro;
        private bool _bExcluir;


        #endregion

        #region | Propriedades

        public int idEndereco
        {
            get { return _idEndereco; }
            set { _idEndereco = value; }
        }
        public int idCliente
        {
            get { return _idCliente; }
            set { _idCliente = value; }
        }
        public int idTipoEndereco
        {
            get { return _idTipoEndereco; }
            set { _idTipoEndereco = value; }

        }
        public string sDscTipoEndereco
        {
            get { return _sDscTipoEndereco; }
            set { _sDscTipoEndereco = value; }
        }
        public string sCEP
        {
            get { return _sCEP; }
            set { _sCEP = value; }
        }
        public string sLogradouro
        {
            get { return _sLogradouro; }
            set { _sLogradouro = value; }
        }
        public string sNumero
        {
            get { return _sNumero; }
            set { _sNumero = value; }
        }
        public string sComplemento
        {
            get { return _sComplemento; }
            set { _sComplemento = value; }
        }
        public string sBairro
        {
            get { return _sBairro; }
            set { _sBairro = value; }
        }
        public string sCidade
        {
            get { return _sCidade; }
            set { _sCidade = value; }
        }
        public string sEstado
        {
            get { return _sEstado; }
            set { _sEstado = value; }
        }
        public string sPais
        {
            get { return _sPais; }
            set { _sPais = value; }
        }
        public string sEnderecoCompleto
        {
            get { return _sEnderecoCompleto; }
            set { _sEnderecoCompleto = value; }
        }

        public string sEnderecoEstrangeiro
        {
            get { return _sEnderecoEstrangeiro; }
            set { _sEnderecoEstrangeiro = value; }
        }

        public bool bExcluir
        {
            get { return _bExcluir; }
            set { _bExcluir = value; }
        }

        #endregion

    }

    [Serializable]
    public class cls_Clientes_Bancario
    {

        #region | Construtor 
        public cls_Clientes_Bancario()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Clientes_Bancario
        (
              int idBancario
            , string sCompany
            , string sCoutry
            , string sBank
            , string sSwift
            , string sAba
            , string sAccount
            , string sEndereco
        )
        {
            _idBancario = idBancario;
            _sCompany = sCompany;
            _sCoutry = sCoutry;
            _sBank = sBank;
            _sSwift = sSwift;
            _sAba = sAba;
            _sAccount = sAccount;
            _sEndereco = sEndereco;
        }

        #endregion

        #region | Membros Privados 

        private int _idBancario;
        private string _sCompany;
        private string _sCoutry;
        private string _sBank;
        private string _sSwift;
        private string _sAba;
        private string _sAccount;
        private string _sEndereco;

        #endregion

        #region | Propriedades

        public int idBancario
        {
            get { return _idBancario; }
            set { _idBancario = value; }
        }

        public string sCompany
        {
            get { return _sCompany; }
            set { _sCompany = value; }
        }

        public string sCoutry
        {
            get { return _sCoutry; }
            set { _sCoutry = value; }
        }

        public string sBank
        {
            get { return _sBank; }
            set { _sBank = value; }
        }

        public string sSwift
        {
            get { return _sSwift; }
            set { _sSwift = value; }
        }

        public string sAba
        {
            get { return _sAba; }
            set { _sAba = value; }
        }

        public string sAccount
        {
            get { return _sAccount; }
            set { _sAccount = value; }
        }

        public string sEndereco
        {
            get { return _sEndereco; }
            set { _sEndereco = value; }
        }
        public int idMoeda { get; set; }
        public string sDscTipoMoeda { get; set; }

        public bool bExcluir { get; set; } = true;
        #endregion

    }

    [Serializable]
    public class cls_TipoRequisicao
    {
        #region Construtor

        public cls_TipoRequisicao()
        {

        }

        public cls_TipoRequisicao
        (
            int idTipoRequisicao,
            int idUsuario,
            string sDscUsuario
        )
        {
            _idTipoRequisicao = idTipoRequisicao;
            _idUsuario = idUsuario;
            _sDscUsuario = sDscUsuario;
        }

        #endregion

        #region Membros Privados

        private int _idTipoRequisicao;
        private int _idUsuario;
        private string _sDscUsuario;


        #endregion

        #region Propriedades

        public int idTipoRequisicao
        {
            get { return _idTipoRequisicao; }
            set { _idTipoRequisicao = value; }
        }

        public int idUsuario
        {
            get { return _idUsuario; }
            set { _idUsuario = value; }
        }

        public string sDscUsuario
        {
            get { return _sDscUsuario; }
            set { _sDscUsuario = value; }
        }

        public int IdTipoMembro { get; set; }
        public int SDscTipoMembro { get; set; }
        #endregion
    }

    [Serializable]
    public class cls_Servicos
    {
        #region Construtor

        public cls_Servicos()
        {

        }

        public cls_Servicos
        (
            int idServico
            ,string sCodigoServico
            ,string sDscServico
            ,string sUnidade
        )
        {
            _idServico = idServico;
            _sCodigoServico = sCodigoServico;
            _sDscServico = sDscServico;
            _sUnidade = sUnidade;
        }

        #endregion

        #region Membros Privados

        private int _idServico;
        private string _sFuncao;
        private string _sCodigoServico;
        private string _sDscServico;
        private int _idTipoServico;
        private string _sDscTipoServico;
        private string _sUnidade;

        #endregion

        #region Propriedades

        public int idServico
        {
            get { return _idServico; }
            set { _idServico = value; }
        }

        public int idTipoServico
        {
            get { return _idTipoServico; }
            set { _idTipoServico = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        public string sCodigoServico
        {
            get { return _sCodigoServico; }
            set { _sCodigoServico = value; }
        }

        public string sDscServico
        {
            get { return _sDscServico; }
            set { _sDscServico = value; }
        }

        public string sUnidade
        {
            get { return _sUnidade; }
            set { _sUnidade = value; }
        }

        public string sDscTipoServico
        {
            get { return _sDscTipoServico; }
            set { _sDscTipoServico = value; }
        }
	  
        #endregion
    }

    [Serializable]
    public class cls_Servico_Recursos
    {
        #region Construtor

        public cls_Servico_Recursos()
        {

        }

        public cls_Servico_Recursos
        (
            int idServico_Recurso
            , string sCodigoServico_Recurso
            , string sDscServico_Recurso
            , string sUnidade
        )
        {
            _idServico_Recurso = idServico_Recurso;
            _sCodigoServico_Recurso = sCodigoServico_Recurso;
            _sDscServico_Recurso = sDscServico_Recurso;
            _sUnidade = sUnidade;
        }

        #endregion

        #region Membros Privados

        private int _idServico_Recurso;
        private string _sFuncao;
        private string _sCodigoServico_Recurso;
        private string _sDscServico_Recurso;
        private int _idTipoServico_Recurso;
        private string _sDscTipoServico_Recurso;
        private string _sUnidade;

        #endregion

        #region Propriedades

        public int idServico_Recurso
        {
            get { return _idServico_Recurso; }
            set { _idServico_Recurso = value; }
        }

        public int idTipoServico_Recurso
        {
            get { return _idTipoServico_Recurso; }
            set { _idTipoServico_Recurso = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        public string sCodigoServico_Recurso
        {
            get { return _sCodigoServico_Recurso; }
            set { _sCodigoServico_Recurso = value; }
        }

        public string sDscServico_Recurso
        {
            get { return _sDscServico_Recurso; }
            set { _sDscServico_Recurso = value; }
        }

        public string sUnidade
        {
            get { return _sUnidade; }
            set { _sUnidade = value; }
        }

        public string sDscTipoServico_Recurso
        {
            get { return _sDscTipoServico_Recurso; }
            set { _sDscTipoServico_Recurso = value; }
        }

        #endregion
    }

    [Serializable]
    public class Cls_Empresas
    {
        #region Construtor

        public Cls_Empresas()
        {

        }

        public Cls_Empresas
        (
            string sImpostos
            , int idLinha
            , int idImpostos
            , decimal nPis
            , decimal nCofins
            , decimal nICMS
            , decimal nCSSL
            , decimal nIRPJ
            , string sFuncao
            , decimal nAno
            , int idRegistro
        )
        {
            _sImpostos = sImpostos;
            _idLinha = idLinha;
            _idImpostos = idImpostos;
            _nPis = nPis;
            _nCofins = nCofins;
            _nICMS = nICMS;
            _nCSSL = nCSSL;
            _nIRPJ = nIRPJ;
            _sFuncao = sFuncao;
            _nAno = nAno;
            _idRegistro = idRegistro;
        }

        #endregion

        #region Membros Privados

        private string _sImpostos;
        private int _idLinha;
        private int _idImpostos;
        private decimal _nPis;
        private decimal _nCofins;
        private decimal _nICMS;
        private decimal _nCSSL;
        private decimal _nIRPJ;
        private string _sFuncao;
        private decimal _nAno;
        private int _idRegistro;
        #endregion

        #region Propriedades

        public string sImpostos
        {
            get { return _sImpostos; }
            set { _sImpostos = value; }
        }
        public int idLinha
        {
            get { return _idLinha; }
            set { _idLinha = value; }
        }
        public int idImpostos
        {
            get { return _idImpostos; }
            set { _idImpostos = value; }
        }
        public decimal nPis
        {
            get { return _nPis; }
            set { _nPis = value; }
        }
        public decimal nCofins
        {
            get { return _nCofins; }
            set { _nCofins = value; }
        }
        public decimal nICMS
        {
            get { return _nICMS; }
            set { _nICMS = value; }
        }
        public decimal nCSSL
        {
            get { return _nCSSL; }
            set { _nCSSL = value; }
        }
        public decimal nIRPJ
        {
            get { return _nIRPJ; }
            set { _nIRPJ = value; }
        }
        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }
        public decimal nAno
        {
            get { return _nAno; }
            set { _nAno = value; }
        }
        public int idRegistro
        {
            get { return _idRegistro; }
            set { _idRegistro = value; }
        }
        #endregion
    }

    [Serializable]
    public class Cls_STSO
    {
        #region Construtor

        public Cls_STSO()
        {

        }

        public Cls_STSO
        (
            int idContador
            , int idUsuario
            , string sFuncao
            , string sEndereco
            , string sUsuario
            , string sObservacao
            , string sSenha
            , string sDscTipo
            , int idTipo
        )
        {
            _idContador = idContador;
            _idUsuario = idUsuario;
            _sEndereco = sEndereco;
            _sUsuario = sUsuario;
            _sObservacao = sObservacao;
            _sFuncao = sFuncao;
            _sSenha = sSenha;
            _idTipo = idTipo;
            _sDscTipo = sDscTipo;
        }

        #endregion

        #region Membros Privados

        private int _idContador;
        private int _idUsuario;
        private string _sEndereco;
        private string _sUsuario;
        private string _sObservacao;
        private string _sFuncao;
        private string _sSenha;
        private string _sDscTipo;
        private int _idTipo;
        #endregion

        #region Propriedades

        public int idContador
        {
            get { return _idContador; }
            set { _idContador = value; }
        }
        public int idUsuario
        {
            get { return _idUsuario; }
            set { _idUsuario = value; }
        }
        public string sEndereco
        {
            get { return _sEndereco; }
            set { _sEndereco = value; }
        }
        public string sUsuario
        {
            get { return _sUsuario; }
            set { _sUsuario = value; }
        }
        public string sObservacao
        {
            get { return _sObservacao; }
            set { _sObservacao = value; }
        }
        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }
        public string sSenha
        {
            get { return _sSenha; }
            set { _sSenha = value; }
        }
        public int idTipo
        {
            get { return _idTipo; }
            set { _idTipo = value; }
        }
        public string sDscTipo
        {
            get { return _sDscTipo; }
            set { _sDscTipo = value; }
        }
        #endregion
    }
}
