using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Flow.FrameWork
{
    [Serializable]
    
    public class cls_OSAcomodacao_Itens
    {

        #region | Construtor 
        public cls_OSAcomodacao_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_OSAcomodacao_Itens
        (
              int idOrdemServico
            , string idTipoAcomodacao
            , string sDscTipoAcomodacao
            , string sNomeAcomodacao
            , string sEnderecoAcomodacao
            , string sNomeContatoAcomodacao
            , string sContatoAcomodacao
        )
        {
            _idOrdemServico = idOrdemServico;
            _idTipoAcomodacao = idTipoAcomodacao;
            _sDscTipoAcomodacao = sDscTipoAcomodacao;
            _sNomeAcomodacao = sNomeAcomodacao;
            _sEnderecoAcomodacao = sEnderecoAcomodacao;
            _sNomeContatoAcomodacao = sNomeContatoAcomodacao;
            _sContatoAcomodacao = sContatoAcomodacao;
        }

        #endregion

        #region | Membros Privados 


        private int _idOrdemServico;
        private string _idTipoAcomodacao;
        private string _sDscTipoAcomodacao;
        private string _sNomeAcomodacao;
        private string _sEnderecoAcomodacao;
        private string _sNomeContatoAcomodacao;
        private string _sContatoAcomodacao;

        #endregion

        #region | Propriedades

        public int idOrdemServico
        {
            get { return _idOrdemServico; }
            set { _idOrdemServico = value; }
        }

        public string idTipoAcomodacao
        {
            get { return _idTipoAcomodacao; }
            set { _idTipoAcomodacao = value; }
        }

        public string sDscTipoAcomodacao
        {
            get { return _sDscTipoAcomodacao; }
            set { _sDscTipoAcomodacao = value; }
        }

        public string sNomeAcomodacao
        {
            get { return _sNomeAcomodacao; }
            set { _sNomeAcomodacao = value; }
        }


        public string sEnderecoAcomodacao
        {
            get { return _sEnderecoAcomodacao; }
            set { _sEnderecoAcomodacao = value; }
        }

        public string sNomeContatoAcomodacao
        {
            get { return _sNomeContatoAcomodacao; }
            set { _sNomeContatoAcomodacao = value; }
        }

        public string sContatoAcomodacao
        {
            get { return _sContatoAcomodacao; }
            set { _sContatoAcomodacao = value; }
        }

        #endregion

    }
}