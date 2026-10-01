<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Recursos.ascx.cs" Inherits="TT_Flow.App.Controles.Recursos" %>

<script type="text/javascript">
    function OnTreeClick(evt) {
        var src = window.event != window.undefined ? window.event.srcElement : evt.target;
        var isChkBoxClick = (src.tagName.toLowerCase() == "input" && src.type == "checkbox");

        if (isChkBoxClick) {
            var parentTable = GetParentByTagName("table", src);
            var nxtSibling = parentTable.nextSibling;

            // Check if nxt sibling is not null & is an element node
            if (nxtSibling && nxtSibling.nodeType == 1) {
                // if node has children    
                if (nxtSibling.tagName.toLowerCase() == "div") {
                    // Check or uncheck children at all levels
                    CheckUncheckChildren(parentTable.nextSibling, src.checked);
                }
            }

            // Check or uncheck parents at all levels
            CheckUncheckParents(src, src.checked);
        }
    }

    function CheckUncheckChildren(childContainer, check) {
        var childChkBoxes = childContainer.getElementsByTagName("input");
        var childChkBoxCount = childChkBoxes.length;
        for (var i = 0; i < childChkBoxCount; i++) {
            childChkBoxes[i].checked = check;
        }
    }

    function CheckUncheckParents(srcChild, check) {
        var parentDiv = GetParentByTagName("div", srcChild);
        var parentNodeTable = parentDiv.previousSibling;

        if (parentNodeTable) {
            var checkUncheckSwitch;

            // Checkbox checked 
            if (check) {
                var isAllSiblingsChecked = AreAllSiblingsChecked(srcChild);
                if (isAllSiblingsChecked)
                    checkUncheckSwitch = true;
                else
                    return;
            }
            else // Checkbox unchecked
            {
                checkUncheckSwitch = false;
            }

            var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");
            if (inpElemsInParentTable.length > 0) {
                var parentNodeChkBox = inpElemsInParentTable[0];
                parentNodeChkBox.checked = checkUncheckSwitch;
                // do the same recursively
                CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);
            }
        }
    }

    function AreAllSiblingsChecked(chkBox) {
        var parentDiv = GetParentByTagName("div", chkBox);
        var childCount = parentDiv.childNodes.length;
        for (var i = 0; i < childCount; i++) {
            if (parentDiv.childNodes[i].nodeType == 1) {
                // check if the child node is an element node
                if (parentDiv.childNodes[i].tagName.toLowerCase() == "table") {
                    var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                    // if any of sibling nodes are not checked, return false
                    if (!prevChkBox.checked) {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    // Utility function to get the container of an element by tagname
    function GetParentByTagName(parentTagName, childElementObj) {
        var parent = childElementObj.parentNode;
        while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) {
            parent = parent.parentNode;
        }
        return parent;
    }
</script>

<style>
    .invisivel {
        display: none;
    }
</style>

<div class="form-group">
    <label>Permissões</label>
    <asp:LinkButton runat="server" ID="cmdAbrirTodos" class="btn btn-xs btn-info" Style="margin-left: 10px;" data-toggle="tooltip" title="Abrir Todos" OnClick="cmdTodos_Click"><i class="fa fa-plus"></i></asp:LinkButton>
    <asp:LinkButton runat="server" ID="cmdFecharTodos" class="btn btn-xs btn-info invisivel" Style="margin-left: 10px;" data-toggle="tooltip" title="Fechar Todos" OnClick="cmdTodos_Click"><i class="fa fa-minus"></i></asp:LinkButton>
    <asp:TreeView ID="tv" runat="server" ShowCheckBoxes="All" ShowLines="True" class="arvore"></asp:TreeView>
</div>
