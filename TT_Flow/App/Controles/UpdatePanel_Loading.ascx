<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdatePanel_Loading.ascx.cs" Inherits="TT_Flow.App.Controles.UpdatePanel_Loading" %>

<style>
    @keyframes rotate {
        from {
            transform: rotate(0deg);
        }

        to {
            transform: rotate(21600deg);
        }
    }

    .modal {
        display: flex;
        position: fixed;
        z-index: 100000000000000000;
        width: 100%;
        height: 100vh;
        overflow: hidden;
        background-color: rgba(0,0,0,0.3);
        text-align: center;
        align-content: center;
        align-items: center;
        justify-content: center;
        justify-items: center;
        font-size: 50px;
        flex-wrap: wrap;
    }

    .imgModal {
        max-width: 150px;
        max-height: 170px;
        animation: rotate 180s linear infinite;
    }
</style>

<asp:UpdateProgress runat="server" ID="UpProgress">
    <ProgressTemplate>
        <div id="UpdProgress_Modal" class="modal">
            <div style="width: 100%;">
                <asp:Image ImageUrl="~\App\img\LogoTT.png" runat="server" ID="imgModal" CssClass="imgModal"/>
            </div>
            <br />
            <b>Aguarde...</b>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
