const uri = '/api/User';
let users = [];

function parseToken() {
    const token = localStorage.getItem('token');
    if (!token) return null;
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload;
    } catch (e) {
        return null;
    }
}

function getHeaders() {
    const headers = {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    };
    const token = localStorage.getItem('token');
    if (token) {
        headers['Authorization'] = 'Bearer ' + token;
    }
    return headers;
}

function getItems() {
    const payload = parseToken();
    let fetchUri = uri;
    if (payload && payload.usertype && payload.usertype !== 'Admin') {
        // regular users should fetch only their own user
        fetchUri = uri + '/me';
    }

    fetch(fetchUri, {
        headers: getHeaders()
    })
        .then(response => {
            if (!response.ok) {
                if (response.status === 401) {
                    localStorage.removeItem('token');
                    window.location.href = '../login.html';
                    return;
                }
                throw new Error('HTTP error ' + response.status);
            }
            return response.json();
        })
        .then(data => displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');
    const addAge = document.getElementById('add-age');
    const addGender = document.getElementById('add-gender');
    const addPassword = document.getElementById('add-password');

    const user = {
        name: addNameTextbox.value.trim(),
        age: parseInt(addAge.value),
        gender: addGender.value,
        password: addPassword.value
    };


    fetch(uri, {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify(user)
        })
        .then(response => {
            if (!response.ok) {
                if (response.status === 401) {
                    localStorage.removeItem('token');
                    window.location.href = '../login.html';
                    return;
                }
                throw new Error('HTTP error ' + response.status);
            }
            return response.json();
        })
        .then(() => {
            getItems();
            addNameTextbox.value = '';
            addAge.value = '';
            addGender.value = '';
            addPassword.value = '';
        })
        .catch(error => console.error('Unable to add user.', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
            method: 'DELETE',
            headers: getHeaders()
        })
        .then(response => {
            if (!response.ok) {
                if (response.status === 401) {
                    localStorage.removeItem('token');
                    window.location.href = '../login.html';
                    return;
                }
                throw new Error('HTTP error ' + response.status);
            }
            return response;
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete user.', error));
}

function displayEditForm(id) {
    const user = users.find(u => u.id === id);

    document.getElementById('edit-id').value = user.id;
    document.getElementById('edit-name').value = user.name;
    document.getElementById('edit-age').value = user.age;
    document.getElementById('edit-gender').value = user.gender;
    document.getElementById('edit-password').value = user.password;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const userId = document.getElementById('edit-id').value.trim();

    const user = {
        name: document.getElementById('edit-name').value.trim(),
        age: parseInt(document.getElementById('edit-age').value),
        gender: document.getElementById('edit-gender').value,
        password: document.getElementById('edit-password').value
    };


    fetch(`${uri}/${userId}`, {
            method: 'PUT',
            headers: getHeaders(),
            body: JSON.stringify(user)
        })
        .then(response => {
            if (!response.ok) {
                if (response.status === 401) {
                    localStorage.removeItem('token');
                    window.location.href = '../login.html';
                    return;
                }
                throw new Error('HTTP error ' + response.status);
            }
            return response;
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to update user.', error));

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function displayCount(userCount) {
    const name = (userCount === 1) ? 'user:' : 'users :';

    document.getElementById('counter').innerText = `${userCount} ${name}`;
}


function displayItems(data) {
    const tBody = document.getElementById('user');
    tBody.innerHTML = '';

    // normalize data to array
    const rows = Array.isArray(data) ? data : [data];

    displayCount(rows.length);

    const button = document.createElement('button');

    const payload = parseToken();
    const currentUserId = payload && payload.userid ? parseInt(payload.userid) : null;
    const isAdmin = payload && payload.usertype && payload.usertype === 'Admin';

    // hide add form for non-admins
    const addForm = document.getElementById('addUserForm');
    if (addForm) {
        addForm.style.display = isAdmin ? 'block' : 'none';
    }

    rows.forEach(user => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        // allow edit for admins or for the current user
        if (isAdmin || (currentUserId !== null && user.id === currentUserId)) {
            editButton.setAttribute('onclick', `displayEditForm(${user.id})`);
        } else {
            editButton.disabled = true;
            editButton.style.opacity = '0.5';
        }

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        // only admins can delete users
        if (isAdmin) {
            deleteButton.setAttribute('onclick', `deleteItem(${user.id})`);
        } else {
            deleteButton.style.display = 'none';
        }

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(user.id);
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNode2 = document.createTextNode(user.name);
        td2.appendChild(textNode2);

        let td3 = tr.insertCell(2);
        let textNode3 = document.createTextNode(user.age);
        td3.appendChild(textNode3);

        let td4 = tr.insertCell(3);
        let textNode4 = document.createTextNode(user.gender);
        td4.appendChild(textNode4);

        let td5 = tr.insertCell(4);
        td5.appendChild(editButton);
        td5.appendChild(deleteButton);
    });

    users = rows;
}