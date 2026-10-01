//variables
// Verificar se as variáveis já foram declaradas para evitar múltiplas declarações
if (typeof cardBeignDragged === 'undefined') {
    var cardBeignDragged;
}
if (typeof dropzones === 'undefined') {
    var dropzones = document.querySelectorAll('.dropzone');
}
if (typeof priorities === 'undefined') {
    var priorities;
}

if (typeof dataColors === 'undefined') {
    var dataColors;
}

if (typeof dataCards === 'undefined') {
    var dataCards = {
        config: {
            maxid: 0
        },
        cards: []
    };
}

//initialize

$(document).ready(() => {

    //localStorage.clear();

    //initializeBoards(dataColors);
    initializeComponents(dataCards);

    //if (JSON.parse(localStorage.getItem('@kanban:data'))) {
    //    dataCards = JSON.parse(localStorage.getItem('@kanban:data'));
    //    console.log("iniciando do localstorage");
    //    initializeComponents(dataCards);
    //}
    //else {
    //    console.log("iniciando do banco");
    //    initializeComponents(dataCards);
    //}

    //if (JSON.parse(localStorage.getItem('@kanban:board'))) {
    //    dataColors = JSON.parse(localStorage.getItem('@kanban:board'));
    //    initializeBoards(dataColors);
    //}
    //else {
    //    initializeBoards(dataColors);
    //}

    initializeCards();
    setupDropzones();

    //$('#add').click(() => {
    //    const title = $('#titleInput').val() !== '' ? $('#titleInput').val() : null;
    //    const description = $('#descriptionInput').val() !== '' ? $('#descriptionInput').val() : null;
    //    $('#titleInput').val('');
    //    $('#descriptionInput').val('');
    //    if (title && description) {
    //        let id = dataCards.config.maxid + 1;
    //        const newCard = {
    //            id,
    //            title,
    //            description,
    //            position: "yellow",
    //            priority: false
    //        }
    //        dataCards.cards.push(newCard);
    //        dataCards.config.maxid = id;
    //        save();
    //        appendComponents(newCard);
    //        initializeCards();
    //    }
    //});
    //$("#deleteAll").click(() => {
    //    dataCards.cards = [];
    //    save();
    //});

});

//functions

function initializeBoards(dataColors) {
    console.log("Recebido em initializeBoards:", dataColors);
    let boardsContainer = document.getElementById("boardsContainer");
    boardsContainer.innerHTML = '';
    dataColors.forEach(item => {
        let htmlString = `
        <div class="board">
            <h3 class="text-center">${item.title.toUpperCase()}</h3>
            <div class="dropzone" data-id="${item.id}" data-color="${item.color}">
                
            </div>
        </div>
        `;
        boardsContainer.innerHTML += htmlString;
    });
    let dropzones = document.querySelectorAll('.dropzone');
    dropzones.forEach(dropzone => {
        dropzone.addEventListener('dragenter', dragenter);
        dropzone.addEventListener('dragover', dragover);
        dropzone.addEventListener('dragleave', dragleave);
        dropzone.addEventListener('drop', drop);
    });
}

function initializeCards() {
    cards = document.querySelectorAll('.kanbanCard');

    cards.forEach(card => {
        card.addEventListener('dragstart', dragstart);
        card.addEventListener('drag', drag);
        card.addEventListener('dragend', dragend);
    });
}

function initializeComponents(dataArray) {
    console.log(dataArray);
    //create all the stored cards and put inside of the todo area      

    dataArray.cards.forEach(card => {
        appendComponents(card);
    })
}


function appendComponents(card) {
    let date = "";
    let time = "";

    if (card.proximoContato) {
        [date, time] = card.proximoContato.split(' ');
    }
    let htmlString = `
         <div id="${card.id.toString()}" class="kanbanCard ${card.color}" draggable="true" data-dropzone-id="${card.position}">
            <div class="content"> 
                <div class="header">
                    <h4 class="title">${card.title}</h4>
                    <div class="proximoContato">
                         ${date}<br>
                         ${time}
                    </div>
                </div>
                <h3 class="cliente">${card.cliente}</h3>
                <div class="footer">
                    <div class="valor">${card.valor}</div>
                    <div class="chance">${card.chance}</div>
                </div>
            </div>                           
            <button class="card-button" data-card-id="${card.id.toString()}" onclick="openModal(this)">Detalhes</button>            
        </div>
    `
    if (card.position) {
        $(`[data-id=${card.position}]`).append(htmlString);
    } else {
        console.log("Invalid card position:", card.id);
    }

    priorities = document.querySelectorAll(".priority");
    setupDraggableCards();
    save();
}
function setupDraggableCards() {
    let cards = document.querySelectorAll('.kanbanCard');
    cards.forEach(card => {
        card.removeEventListener('dragstart', dragstart);
        card.removeEventListener('drag', drag);
        card.removeEventListener('dragend', dragend);

        card.addEventListener('dragstart', dragstart);
        card.addEventListener('drag', drag);
        card.addEventListener('dragend', dragend);
    });
}

function togglePriority(event) {
    event.target.classList.toggle("is-priority");
    dataCards.cards.forEach(card => {
        if (event.target.id.split('-')[1] === card.id.toString()) {
            card.priority = card.priority ? false : true;
        }
    })
    save();
}

function deleteCard(id) {
    dataCards.cards.forEach(card => {
        if (card.id === id) {
            let index = dataCards.cards.indexOf(card);
            console.log(index)
            dataCards.cards.splice(index, 1);
            console.log(dataCards.cards);
            save();
        }
    })
}

function removeClasses(cardBeignDragged, color) {
    cardBeignDragged.classList.remove('_danger');
    cardBeignDragged.classList.remove('_primary');
    cardBeignDragged.classList.remove('_warning');
    cardBeignDragged.classList.remove('_success');
    cardBeignDragged.classList.remove('_info');
    cardBeignDragged.classList.remove('_default');
    cardBeignDragged.classList.add(color);
    position(cardBeignDragged, color);
}

function save() {
    // localStorage.setItem('@kanban:data', JSON.stringify(dataCards));
}

function saveBoard() {
    // localStorage.setItem('@kanban:board', JSON.stringify(dataColors));
}

function position(cardBeignDragged, dropzoneId) {
    const index = dataCards.cards.findIndex(card => card.id === parseInt(cardBeignDragged.id));
    if (index === -1) {
        console.error('Card not found: could not update position.');
        return;
    }
    dataCards.cards[index].position = dropzoneId;
    save();
}

//cards
function dragstart(event) {
    dropzones.forEach(dropzone => dropzone.classList.add('highlight'));
    this.classList.add('is-dragging');
    event.dataTransfer.setData("text/plain", this.id);

    const initialPosition = this.closest('.dropzone')?.dataset.id;
    this.setAttribute('data-initial-position', initialPosition);
}

function drag() {

}

function dragend(event) {
    const initialDropzone = document.querySelector(`[data-id="${this.getAttribute('data-initial-position')}"]`);
    const initialPosition = initialDropzone ? initialDropzone.getAttribute('data-id') : null;

    const finalDropzone = this.closest('.dropzone');
    const finalPosition = finalDropzone ? finalDropzone.getAttribute('data-id') : null;
    const cardId = this.id;
    console.log('Moveu card: ', cardId, ' de ', initialPosition, ' para ', finalPosition);

    dropzones.forEach(dropzone => dropzone.classList.remove('highlight'));
    this.classList.remove('is-dragging');

    if (finalDropzone) {
        this.dataset.dropzoneId = finalPosition;
        removeClasses(this, finalDropzone.dataset.color);
        position(this, finalPosition);
    }

    sendPositionsToServer(cardId, initialPosition, finalPosition);
    this.removeAttribute('data-initial-position');
}

// Release cards area
function dragenter() {

}

function dragover(event) {
    event.preventDefault();
    this.classList.add('over');
    cardBeignDragged = document.querySelector('.is-dragging');

    const afterElement = getDragAfterElement(this, event.clientY);
    if (afterElement == null) {
        this.appendChild(cardBeignDragged);
    } else {
        this.insertBefore(cardBeignDragged, afterElement);
    }
}

function dragleave() {

    this.classList.remove('over');
}

function drop() {
    this.classList.remove('over');
}

function setupDropzones() {
    let dropzones = document.querySelectorAll('.dropzone');
    dropzones.forEach(dropzone => {
        dropzone.addEventListener('dragover', dragover);
        dropzone.addEventListener('drop', drop);
    });
}

function getDragAfterElement(dropzone, y) {
    const draggableElements = [...dropzone.querySelectorAll('.kanbanCard:not(.is-dragging)')];

    return draggableElements.reduce((closest, child) => {
        const box = child.getBoundingClientRect();
        const offset = y - box.top - box.height / 2;
        if (offset < 0 && offset > closest.offset) {
            return { offset: offset, element: child };
        } else {
            return closest;
        }
    }, { offset: Number.NEGATIVE_INFINITY }).element;
}

function sendPositionsToServer(cardId, initialPosition, finalPosition) {

    $.ajax({
        url: '/app/Paginas/Comercial/CRM.aspx/AlterarStatusKanban',
        data: JSON.stringify({
            sCardId: cardId.toString(),
            sPosicaoInicial: initialPosition,
            sPosicaoFinal: finalPosition
        }),
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            console.log(data);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}


//function openModal(button) {
//    console.log("chego aqui");
//    const cardId = button.getAttribute('data-card-id');
//    console.log("Card ID: " + cardId);

//    console.log("hddIdCard ID: <%= hddIdCard.ClientID %>");
//    console.log("btnTriggerModalKanban ID: <%= btnTriggerModalKanban.ClientID %>");
//    var hddIdCard = document.getElementById('<%= hddIdCard.ClientID %>');
//    var btnTrigger = document.getElementById('<%= btnTriggerModalKanban.ClientID %>');

//    if (hddIdCard && btnTrigger) {
//        hddIdCard.value = cardId;
//        btnTrigger.click();
//    } else {
//        console.error("Elemento(s) não encontrado(s). Verifique os IDs e tente novamente.");
//    }
//}
