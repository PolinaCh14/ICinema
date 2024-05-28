document.getElementById('togglePassword').addEventListener('click', function () {
    const passwordField = document.getElementById('password-field');
    const passwordFieldType = passwordField.getAttribute('type');
    
    if (passwordFieldType === 'password') {
        passwordField.setAttribute('type', 'text');
        this.innerHTML = `<img src="../images/icons/eye-invisible.png" class="eye-button"/>`;
    } else {
        passwordField.setAttribute('type', 'password');
        this.innerHTML = `<img src="../images/icons/eye-visible.png" class="eye-button"/>`;
    }
});

document.getElementById('togglePasswordConfirm').addEventListener('click', function () {
    const passwordField = document.getElementById('confirm-password-field');
    const passwordFieldType = passwordField.getAttribute('type');

    if (passwordFieldType === 'password') {
        passwordField.setAttribute('type', 'text');
        this.innerHTML = `<img src="../images/icons/eye-invisible.png" class="eye-button"/>`;
    } else {
        passwordField.setAttribute('type', 'password');
        this.innerHTML = `<img src="../images/icons/eye-visible.png" class="eye-button"/>`;
    }
});