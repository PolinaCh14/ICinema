const body = document.querySelector("body");
const header = document.querySelector(".header");
const menuButton = document.querySelector(".header-nav__menu-button");
const navLinks = document.querySelector(".header-nav__menu");

menuButton.addEventListener("click", () => {
    body.classList.toggle("fixed");
    header.classList.toggle("responsive");
});

navLinks.addEventListener("click", () => {
    body.classList.remove("fixed");
    header.classList.remove("responsive");
})