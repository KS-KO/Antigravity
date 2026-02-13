/**
 * Global Picks Premium Landing Page JS
 * - Handles Header Scroll Interaction
 * - Subtle animations and logic
 */

document.addEventListener('DOMContentLoaded', () => {
    const header = document.getElementById('header');

    // Header Scroll Effect
    window.addEventListener('scroll', () => {
        if (window.scrollY > 50) {
            header.classList.add('scrolled');
        } else {
            header.classList.remove('scrolled');
        }
    });

    // Simple interaction for CTA buttons
    const ctaButtons = document.querySelectorAll('.btn-primary');
    ctaButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            alert('글로벌 픽스 쇼핑 경험에 오신 것을 환영합니다! 현재 베타 준비 중입니다.');
        });
    });
});
