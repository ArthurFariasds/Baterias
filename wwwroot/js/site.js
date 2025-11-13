
(function () {
    const THEME_KEY = 'batteryswap_theme'; 
    const root = document.documentElement; 

    function getStoredTheme() {
        try {
            return localStorage.getItem(THEME_KEY);
        } catch (e) {
            return null;
        }
    }

    function storeTheme(theme) {
        try {
            localStorage.setItem(THEME_KEY, theme);
        } catch (e) { /* ignore */ }
    }

    function applyTheme(theme) {
        if (theme === 'light') {
            root.setAttribute('data-theme', 'light');
            root.classList.remove('theme-dark');
            root.classList.add('theme-light');
        } else {
            root.setAttribute('data-theme', 'dark');
            root.classList.remove('theme-light');
            root.classList.add('theme-dark');
        }

        document.querySelectorAll('.form-control, .form-select, textarea').forEach(el => {
            if (theme === 'light') {
                el.classList.remove('form-control-dark');
                el.classList.add('form-control-light');
            } else {
                el.classList.remove('form-control-light');
                el.classList.add('form-control-dark');
            }
        });

        const toggle = document.getElementById('themeToggleIcon');
        if (toggle) {
            toggle.className = theme === 'light' ? 'bi bi-sun-fill' : 'bi bi-moon-fill';
            toggle.setAttribute('title', theme === 'light' ? 'Modo claro' : 'Modo escuro');
        }
    }

    const stored = getStoredTheme();
    if (stored) {
        applyTheme(stored);
    } else {
        const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        applyTheme(prefersDark ? 'dark' : 'light');
    }

    window.toggleTheme = function () {
        const current = root.getAttribute('data-theme') === 'light' ? 'light' : 'dark';
        const next = current === 'light' ? 'dark' : 'light';
        applyTheme(next);
        storeTheme(next);
    };
})();


document.addEventListener('DOMContentLoaded', function () {
    const tipoUsuario = document.getElementById('tipoUsuario');
    const tipoEmpresa = document.getElementById('tipoEmpresa');
    const camposEmpresa = document.getElementById('camposEmpresa');

    function toggleCamposEmpresa() {
        if (tipoEmpresa && tipoEmpresa.checked) {
            camposEmpresa.style.display = 'block';
            addRequiredValidation();
        } else {
            if (camposEmpresa) camposEmpresa.style.display = 'none';
            removeRequiredValidation();
        }
    }

    function addRequiredValidation() {
        const cnpjInput = document.querySelector('input[name="Cnpj"]');
        const enderecoInput = document.querySelector('input[name="Endereco"]');

        if (cnpjInput) cnpjInput.setAttribute('required', 'required');
        if (enderecoInput) enderecoInput.setAttribute('required', 'required');
    }

    function removeRequiredValidation() {
        const cnpjInput = document.querySelector('input[name="Cnpj"]');
        const enderecoInput = document.querySelector('input[name="Endereco"]');

        if (cnpjInput) cnpjInput.removeAttribute('required');
        if (enderecoInput) enderecoInput.removeAttribute('required');

        const cnpjError = cnpjInput?.nextElementSibling;
        const enderecoError = enderecoInput?.nextElementSibling;

        if (cnpjError && cnpjError.classList.contains('text-danger')) {
            cnpjError.textContent = '';
        }
        if (enderecoError && enderecoError.classList.contains('text-danger')) {
            enderecoError.textContent = '';
        }
    }

    if (tipoUsuario && tipoEmpresa && camposEmpresa) {
        tipoUsuario.addEventListener('change', toggleCamposEmpresa);
        tipoEmpresa.addEventListener('change', toggleCamposEmpresa);

        toggleCamposEmpresa();
    }

    const telefoneInput = document.querySelector('input[name="Telefone"]');
    if (telefoneInput) {
        telefoneInput.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, '');

            if (value.length <= 10) {
                value = value.replace(/(\d{2})(\d{4})(\d{0,4})/, '($1) $2-$3');
            } else {
                value = value.replace(/(\d{2})(\d{5})(\d{0,4})/, '($1) $2-$3');
            }

            e.target.value = value;
        });
    }

    const cnpjInput = document.querySelector('input[name="Cnpj"]');
    if (cnpjInput) {
        cnpjInput.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, '');

            if (value.length <= 14) {
                value = value.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{0,2})/, '$1.$2.$3/$4-$5');
            }

            e.target.value = value;
        });
    }

    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            let isValid = true;

            const requiredFields = registerForm.querySelectorAll('[required]');
            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    isValid = false;
                    field.classList.add('is-invalid');

                    let errorElement = field.nextElementSibling;
                    if (!errorElement || !errorElement.classList.contains('text-danger')) {
                        errorElement = document.createElement('div');
                        errorElement.className = 'text-danger small mt-1';
                        field.parentNode.appendChild(errorElement);
                    }
                    errorElement.textContent = 'Este campo é obrigatório.';
                } else {
                    field.classList.remove('is-invalid');
                    field.classList.add('is-valid');

                    const errorElement = field.nextElementSibling;
                    if (errorElement && errorElement.classList.contains('text-danger')) {
                        errorElement.textContent = '';
                    }
                }
            });

            const password = document.querySelector('input[name="Password"]');
            const confirmPassword = document.querySelector('input[name="ConfirmPassword"]');

            if (password && confirmPassword && password.value !== confirmPassword.value) {
                isValid = false;
                confirmPassword.classList.add('is-invalid');

                let errorElement = confirmPassword.nextElementSibling;
                if (!errorElement || !errorElement.classList.contains('text-danger')) {
                    errorElement = document.createElement('div');
                    errorElement.className = 'text-danger small mt-1';
                    confirmPassword.parentNode.appendChild(errorElement);
                }
                errorElement.textContent = 'As senhas não coincidem.';
            }

            if (!isValid) {
                e.preventDefault();

                const firstError = registerForm.querySelector('.is-invalid');
                if (firstError) {
                    firstError.scrollIntoView({
                        behavior: 'smooth',
                        block: 'center'
                    });
                }
            }
        });
    }

    const formInputs = document.querySelectorAll('.form-control');
    formInputs.forEach(input => {
        input.addEventListener('blur', function () {
            if (this.value.trim() && this.checkValidity()) {
                this.classList.add('is-valid');
                this.classList.remove('is-invalid');
            } else if (this.hasAttribute('required') && !this.value.trim()) {
                this.classList.add('is-invalid');
                this.classList.remove('is-valid');
            }
        });

        input.addEventListener('input', function () {
            if (this.classList.contains('is-invalid') && this.value.trim()) {
                this.classList.remove('is-invalid');
            }
        });
    });
});

function formatCNPJ(cnpj) {
    return cnpj.replace(/\D/g, '')
        .replace(/(\d{2})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1/$2')
        .replace(/(\d{4})(\d{1,2})$/, '$1-$2');
}

function formatPhone(phone) {
    const numbers = phone.replace(/\D/g, '');
    if (numbers.length === 11) {
        return numbers.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    } else {
        return numbers.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const agendamentoForm = document.getElementById('agendamentoForm');
    const bateriaSelect = document.getElementById('bateriaSelect');

    if (agendamentoForm) {
        agendamentoForm.addEventListener('submit', function (e) {
            let isValid = true;

            if (!bateriaSelect.value) {
                isValid = false;
                bateriaSelect.classList.add('is-invalid');

                let errorElement = bateriaSelect.nextElementSibling;
                if (!errorElement || !errorElement.classList.contains('text-danger')) {
                    errorElement = document.createElement('div');
                    errorElement.className = 'text-danger small mt-1';
                    bateriaSelect.parentNode.appendChild(errorElement);
                }
                errorElement.textContent = 'Por favor, selecione uma bateria.';
            } else {
                bateriaSelect.classList.remove('is-invalid');
                bateriaSelect.classList.add('is-valid');

                const errorElement = bateriaSelect.nextElementSibling;
                if (errorElement && errorElement.classList.contains('text-danger')) {
                    errorElement.textContent = '';
                }
            }

            try {
                console.log('=== DADOS DO AGENDAMENTO ===');
                console.log('EmpresaId:', document.querySelector('[name="EmpresaId"]').value);
                console.log('TipoBateria:', bateriaSelect.value);
                console.log('Observacoes:', document.querySelector('[name="Observacoes"]').value);
            } catch (e) { /* ignore */ }

            if (!isValid) {
                e.preventDefault();

                const firstError = agendamentoForm.querySelector('.is-invalid');
                if (firstError) {
                    firstError.scrollIntoView({
                        behavior: 'smooth',
                        block: 'center'
                    });
                    firstError.focus();
                }
                return false;
            }
        });
    }

    if (bateriaSelect) {
        bateriaSelect.addEventListener('change', function () {
            if (this.value) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
            }
        });
    }

    const cards = document.querySelectorAll('.card');
    cards.forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-2px)';
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0)';
        });
    });

    if (bateriaSelect) {
        bateriaSelect.addEventListener('focus', function () {
            this.parentElement.classList.add('focused');
        });

        bateriaSelect.addEventListener('blur', function () {
            this.parentElement.classList.remove('focused');
        });
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            let isValid = true;
            const requiredFields = form.querySelectorAll('[required]');

            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    isValid = false;
                    field.classList.add('is-invalid');

                    let errorElement = field.nextElementSibling;
                    if (!errorElement || !errorElement.classList.contains('text-danger')) {
                        errorElement = document.createElement('div');
                        errorElement.className = 'text-danger small mt-1';
                        field.parentNode.appendChild(errorElement);
                    }
                    errorElement.textContent = 'Este campo é obrigatório.';
                } else {
                    field.classList.remove('is-invalid');
                    field.classList.add('is-valid');
                }
            });

            const quantidadeInput = form.querySelector('input[name="Quantidade"]');
            if (quantidadeInput && quantidadeInput.value < 0) {
                isValid = false;
                quantidadeInput.classList.add('is-invalid');

                let errorElement = quantidadeInput.nextElementSibling;
                if (!errorElement || !errorElement.classList.contains('text-danger')) {
                    errorElement = document.createElement('div');
                    errorElement.className = 'text-danger small mt-1';
                    quantidadeInput.parentNode.appendChild(errorElement);
                }
                errorElement.textContent = 'A quantidade não pode ser negativa.';
            }

            if (!isValid) {
                e.preventDefault();

                const firstError = form.querySelector('.is-invalid');
                if (firstError) {
                    firstError.scrollIntoView({
                        behavior: 'smooth',
                        block: 'center'
                    });
                    firstError.focus();
                }
            }
        });
    });

    const formInputs2 = document.querySelectorAll('.form-control, .form-select');
    formInputs2.forEach(input => {
        input.addEventListener('blur', function () {
            if (this.value.trim() && this.checkValidity()) {
                this.classList.add('is-valid');
                this.classList.remove('is-invalid');
            } else if (this.hasAttribute('required') && !this.value.trim()) {
                this.classList.add('is-invalid');
                this.classList.remove('is-valid');
            }
        });

        input.addEventListener('input', function () {
            if (this.classList.contains('is-invalid') && this.value.trim()) {
                this.classList.remove('is-invalid');
            }
        });
    });

    const deleteForm = document.querySelector('form[action*="Delete"]');
    if (deleteForm) {
        deleteForm.addEventListener('submit', function (e) {
            if (!confirm('Tem certeza que deseja excluir esta bateria? Esta ação não pode ser desfeita.')) {
                e.preventDefault();
                return false;
            }
        });
    }

    const firstInput = document.querySelector('form input:not([type="hidden"]), form select, form textarea');
    if (firstInput) {
        firstInput.focus();
    }
});

function previewBateria(formData) {
    console.group('🔋 Preview da Bateria');
    console.log('📋 Dados do Formulário:');
    console.table(formData);
    console.groupEnd();
}

document.addEventListener("DOMContentLoaded", function () {
    const btnEdit = document.getElementById("btnEdit");
    const btnCancel = document.getElementById("btnCancel");
    const viewMode = document.getElementById("viewMode");
    const editMode = document.getElementById("editMode");

    btnEdit?.addEventListener("click", function () {
        viewMode.style.display = "none";
        editMode.style.display = "block";
        const firstInput = editMode.querySelector('input:not([readonly])');
        if (firstInput) firstInput.focus();
    });

    btnCancel?.addEventListener("click", function () {
        editMode.style.display = "none";
        viewMode.style.display = "block";
    });

    const telefoneInput = document.querySelector('input[name="Telefone"]');
    if (telefoneInput) {
        telefoneInput.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, '');

            if (value.length <= 10) {
                value = value.replace(/(\d{2})(\d{4})(\d{0,4})/, '($1) $2-$3');
            } else {
                value = value.replace(/(\d{2})(\d{5})(\d{0,4})/, '($1) $2-$3');
            }

            e.target.value = value;
        });
    }
});
document.addEventListener("DOMContentLoaded", function () {
    const btnEdit = document.getElementById("btnEdit");
    const btnCancel = document.getElementById("btnCancel");
    const viewMode = document.getElementById("viewMode");
    const editMode = document.getElementById("editMode");

    btnEdit?.addEventListener("click", function () {
        viewMode.style.display = "none";
        editMode.style.display = "block";
        const firstInput = editMode.querySelector('input:not([readonly])');
        if (firstInput) firstInput.focus();
    });

    btnCancel?.addEventListener("click", function () {
        editMode.style.display = "none";
        viewMode.style.display = "block";
    });

    const telefoneInput = document.querySelector('input[name="Telefone"]');
    if (telefoneInput) {
        telefoneInput.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, '');

            if (value.length <= 10) {
                value = value.replace(/(\d{2})(\d{4})(\d{0,4})/, '($1) $2-$3');
            } else {
                value = value.replace(/(\d{2})(\d{5})(\d{0,4})/, '($1) $2-$3');
            }

            e.target.value = value;
        });
    }
});
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".alert-close").forEach(btn => {
        btn.addEventListener("click", function () {
            const alert = this.closest(".alert-modern");
            if (alert) {
                // animação opcional de fade
                alert.style.transition = "opacity 0.3s ease, transform 0.3s ease";
                alert.style.opacity = "0";
                alert.style.transform = "translateY(-10px)";
                setTimeout(() => alert.remove(), 300); // remove do DOM após a animação
            }
        });
    });
});
