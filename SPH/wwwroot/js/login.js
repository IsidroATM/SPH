document.addEventListener('DOMContentLoaded', function () {
    // Seleccionamos los elementos del DOM según la nueva estructura
    const inputs = document.querySelectorAll(".input-field");
    const toggle_btn = document.querySelectorAll(".toggle");
    const main = document.querySelector("main");
    const bullets = document.querySelectorAll(".bullets span");
    const images = document.querySelectorAll(".image");
    const textGroup = document.querySelector('.text-group');

    // Manejo de inputs para agregar la clase 'active' cuando están enfocados
    inputs.forEach((inp) => {
        inp.addEventListener("focus", () => {
            inp.classList.add("active");
        });
        inp.addEventListener("blur", () => {
            if (inp.value !== "") return;
            inp.classList.remove("active");
        });
    });

    // Cambio de modo al hacer clic en el botón de 'toggle'
    toggle_btn.forEach((btn) => {
        btn.addEventListener("click", () => {
            main.classList.toggle("sign-up-mode");
        });
    });

    // Función para cambiar las imágenes del carrusel
    function moveSlider() {
        let index = this.dataset.value; // Obtener el valor del índice desde el atributo data-value
        let currentImage = document.querySelector(`.img-${index}`); // Seleccionar la imagen correspondiente

        // Ocultar todas las imágenes y mostrar solo la actual
        images.forEach((img) => img.classList.remove("show"));
        currentImage.classList.add("show");

        // Deslizar el texto correspondiente
        textGroup.style.transform = `translateY(${-(index - 1) * 2.2}rem)`;

        // Actualizar el estado activo de las balas
        bullets.forEach((bull) => bull.classList.remove("active"));
        this.classList.add("active");
    }

    // Asignar el evento de clic a las balas del carrusel
    bullets.forEach((bullet) => {
        bullet.addEventListener("click", moveSlider);
    });

    // Auto-play del carrusel cada 5 segundos (opcional)
    let currentIndex = 0; // Índice inicial
    function autoPlay() {
        currentIndex = (currentIndex + 1) % images.length; // Incrementa el índice y lo reinicia si llega al final
        let currentImage = document.querySelector(`.img-${currentIndex + 1}`);
        images.forEach((img) => img.classList.remove("show"));
        currentImage.classList.add("show");

        // Mover texto y actualizar las balas
        textGroup.style.transform = `translateY(${-(currentIndex) * 2.2}rem)`;
        bullets.forEach((bull) => bull.classList.remove("active"));
        bullets[currentIndex].classList.add("active");
    }

    setInterval(autoPlay, 5000); // Cambia cada 5 segundos
});
