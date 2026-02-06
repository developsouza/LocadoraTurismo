/*!
 * SISTEMA DE LOCAÇÃO E TURISMO - JAVASCRIPT CONSOLIDADO CORRIGIDO
 * Versão com validação integrada e loading overlay corrigido
 * Compatível com ASP.NET Core MVC + Razor + Bootstrap
 */

/* ===== SIDEBAR MANAGER ===== */
class SidebarManager {
    constructor() {
        this.sidebar = document.getElementById('sidebar');
        this.sidebarToggle = document.getElementById('sidebarToggle');
        this.sidebarOverlay = document.getElementById('sidebarOverlay');
        this.isCollapsed = this.getCollapsedState();
        this.isMobile = window.innerWidth <= 768;
        this.init();
    }

    init() {
        if (!this.sidebar) return;
        this.updateSidebarState();
        this.setupEventListeners();
        this.handleResize();
    }

    setupEventListeners() {
        if (this.sidebarToggle) {
            this.sidebarToggle.addEventListener('click', () => this.toggle());
        }
        if (this.sidebarOverlay) {
            this.sidebarOverlay.addEventListener('click', () => this.hide());
        }
        window.addEventListener('resize', () => this.handleResize());
        this.setupDropdowns();
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.isMobile && this.isVisible()) {
                this.hide();
            }
        });
    }

    setupDropdowns() {
        const dropdownLinks = document.querySelectorAll('.nav-dropdown');
        dropdownLinks.forEach(link => {
            link.removeEventListener('click', this.handleDropdownClick);
            const handleClick = (e) => {
                e.preventDefault();
                e.stopPropagation();
                if (this.isCollapsed && !this.isMobile) return;
                const targetId = link.getAttribute('href')?.substring(1);
                const submenu = targetId ? document.getElementById(targetId) : null;
                if (!submenu) return;
                const isCurrentlyOpen = submenu.classList.contains('show');
                dropdownLinks.forEach(otherLink => {
                    if (otherLink !== link) {
                        otherLink.setAttribute('aria-expanded', 'false');
                        otherLink.classList.remove('active');
                        const otherId = otherLink.getAttribute('href')?.substring(1);
                        const otherSubmenu = otherId ? document.getElementById(otherId) : null;
                        if (otherSubmenu) {
                            otherSubmenu.classList.remove('show', 'active');
                        }
                    }
                });
                if (isCurrentlyOpen) {
                    link.setAttribute('aria-expanded', 'false');
                    link.classList.remove('active');
                    submenu.classList.remove('show', 'active');
                } else {
                    link.setAttribute('aria-expanded', 'true');
                    link.classList.add('active');
                    submenu.classList.add('show', 'active');
                }
            };
            link.addEventListener('click', handleClick);
        });
    }

    toggle() {
        this.isMobile ? (this.isCollapsed ? this.show() : this.hide()) : (this.isCollapsed ? this.expand() : this.collapse());
    }

    collapse() {
        this.isCollapsed = true;
        this.updateSidebarState();
        this.saveCollapsedState(true);
        this.closeAllDropdowns();
        this.dispatchEvent('collapsed');
    }

    expand() {
        this.isCollapsed = false;
        this.updateSidebarState();
        this.saveCollapsedState(false);
        this.dispatchEvent('expanded');
    }

    show() {
        if (this.isMobile) {
            this.sidebar?.classList.remove('mobile-hidden');
            this.sidebar?.classList.add('mobile-visible');
            this.sidebarOverlay?.classList.add('active');
            document.body.style.overflow = 'hidden';
            this.dispatchEvent('shown');
        }
    }

    hide() {
        if (this.isMobile) {
            this.sidebar?.classList.add('mobile-hidden');
            this.sidebar?.classList.remove('mobile-visible');
            this.sidebarOverlay?.classList.remove('active');
            document.body.style.overflow = '';
            this.dispatchEvent('hidden');
        }
    }

    isVisible() {
        if (!this.sidebar) return false;
        return this.isMobile ? !this.sidebar.classList.contains('mobile-hidden') : true;
    }

    closeAllDropdowns() {
        document.querySelectorAll('.nav-dropdown').forEach(link => {
            link.setAttribute('aria-expanded', 'false');
            link.classList.remove('active');
        });
        document.querySelectorAll('.nav-submenu').forEach(submenu => {
            submenu.classList.remove('show', 'active');
        });
    }

    updateSidebarState() {
        if (!this.sidebar) return;
        if (this.isMobile) {
            this.sidebar.classList.remove('collapsed');
            this.sidebar.classList.add('mobile-hidden');
        } else {
            this.sidebar.classList.remove('mobile-hidden', 'mobile-visible');
            this.sidebar.classList.toggle('collapsed', this.isCollapsed);
        }
    }

    handleResize() {
        const wasMobile = this.isMobile;
        this.isMobile = window.innerWidth <= 768;
        if (wasMobile !== this.isMobile) {
            if (this.isMobile) {
                this.sidebar?.classList.remove('collapsed');
                this.hide();
            } else {
                this.sidebar?.classList.remove('mobile-hidden', 'mobile-visible');
                this.sidebarOverlay?.classList.remove('active');
                document.body.style.overflow = '';
                this.updateSidebarState();
            }
        }
    }

    getCollapsedState() {
        return localStorage.getItem('sidebar-collapsed') === 'true';
    }

    saveCollapsedState(collapsed) {
        localStorage.setItem('sidebar-collapsed', collapsed.toString());
    }

    dispatchEvent(eventName) {
        const event = new CustomEvent(`sidebar:${eventName}`, { detail: { sidebar: this } });
        document.dispatchEvent(event);
    }
}

/* ===== THEME MANAGER ===== */
class ThemeManager {
    constructor() {
        this.themeToggle = document.getElementById('themeToggle');
        this.themeIcon = document.getElementById('themeIcon');
        this.currentTheme = this.getStoredTheme() || this.getSystemTheme();
        this.init();
    }

    init() {
        this.applyTheme(this.currentTheme);
        this.setupEventListeners();
        this.watchSystemTheme();
    }

    setupEventListeners() {
        if (this.themeToggle) {
            this.themeToggle.addEventListener('click', () => this.toggle());
        }
        window.addEventListener('storage', (e) => {
            if (e.key === 'theme-preference') {
                this.currentTheme = e.newValue || this.getSystemTheme();
                this.applyTheme(this.currentTheme);
            }
        });
    }

    toggle() {
        const newTheme = this.currentTheme === 'light' ? 'dark' : 'light';
        this.setTheme(newTheme);
        if (this.themeToggle) {
            this.themeToggle.style.transform = 'scale(0.9)';
            setTimeout(() => {
                this.themeToggle.style.transform = 'scale(1)';
            }, 150);
        }
    }

    setTheme(theme) {
        this.currentTheme = theme;
        this.applyTheme(theme);
        this.storeTheme(theme);
        this.dispatchEvent('themeChanged', { theme });
    }

    applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        document.body.setAttribute('data-theme', theme);
        this.updateThemeIcon(theme);
        this.updateMetaThemeColor(theme);
    }

    updateThemeIcon(theme) {
        if (!this.themeIcon) return;
        const icon = theme === 'light' ? 'fas fa-moon' : 'fas fa-sun';
        this.themeIcon.className = icon;
        this.themeIcon.style.transform = 'rotate(360deg)';
        setTimeout(() => {
            this.themeIcon.style.transform = 'rotate(0deg)';
        }, 300);
    }

    updateMetaThemeColor(theme) {
        let metaThemeColor = document.querySelector('meta[name="theme-color"]');
        if (!metaThemeColor) {
            metaThemeColor = document.createElement('meta');
            metaThemeColor.name = 'theme-color';
            document.head.appendChild(metaThemeColor);
        }
        metaThemeColor.content = theme === 'light' ? '#6366f1' : '#1e293b';
    }

    getSystemTheme() {
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    getStoredTheme() {
        return localStorage.getItem('theme-preference');
    }

    storeTheme(theme) {
        localStorage.setItem('theme-preference', theme);
    }

    watchSystemTheme() {
        const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
        mediaQuery.addEventListener('change', (e) => {
            if (!this.getStoredTheme()) {
                this.setTheme(e.matches ? 'dark' : 'light');
            }
        });
    }

    dispatchEvent(eventName, detail = {}) {
        const event = new CustomEvent(`theme:${eventName}`, { detail });
        document.dispatchEvent(event);
    }
}

/* ===== NOTIFICATION SYSTEM - VERSÃO COMPLETA ===== */
class NotificationSystem {
    static notificationContainer = null;
    static pollingInterval = null;
    static updateFrequency = 60000; // 1 minuto

    static initialize() {
        this.createContainer();
        this.setupBadgeClick();
        this.startPolling();
    }

    static createContainer() {
        if (!this.notificationContainer) {
            this.notificationContainer = document.createElement('div');
            this.notificationContainer.id = 'notification-container';
            this.notificationContainer.style.cssText = `
                position: fixed;
                top: 80px;
                right: 20px;
                z-index: 9999;
                max-width: 400px;
                pointer-events: none;
            `;
            document.body.appendChild(this.notificationContainer);
        }
    }

    static show(message, type = 'info', duration = 5000) {
        this.createContainer();

        const notification = document.createElement('div');
        notification.className = `alert alert-${type === 'error' ? 'danger' : type} notification-toast animate-slide-in`;
        notification.style.cssText = `
            pointer-events: auto;
            margin-bottom: 10px;
            box-shadow: 0 10px 25px rgba(0,0,0,0.15);
            border: none;
            animation: slideInRight 0.3s ease;
            word-wrap: break-word;
        `;

        const icons = {
            success: 'check-circle',
            danger: 'exclamation-triangle',
            error: 'exclamation-triangle',
            warning: 'exclamation-triangle',
            info: 'info-circle'
        };

        notification.innerHTML = `
            <div class="d-flex align-items-center">
                <i class="fas fa-${icons[type] || icons.info} me-2"></i>
                <div class="flex-grow-1">${message}</div>
                <button type="button" class="btn-close ms-2" onclick="this.parentNode.parentNode.remove()"></button>
            </div>
        `;

        this.notificationContainer.appendChild(notification);

        if (duration > 0) {
            setTimeout(() => {
                notification.style.animation = 'slideOutRight 0.3s ease';
                setTimeout(() => notification.remove(), 300);
            }, duration);
        }

        return notification;
    }

    static success(message, duration) { return this.show(message, 'success', duration); }
    static error(message, duration) { return this.show(message, 'error', duration); }
    static warning(message, duration) { return this.show(message, 'warning', duration); }
    static info(message, duration) { return this.show(message, 'info', duration); }

    static async startPolling() {
        // Carregar notificações imediatamente
        await this.loadNotifications();

        // Configurar polling periódico
        if (this.pollingInterval) {
            clearInterval(this.pollingInterval);
        }

        this.pollingInterval = setInterval(async () => {
            await this.loadNotifications();
        }, this.updateFrequency);
    }

    static stopPolling() {
        if (this.pollingInterval) {
            clearInterval(this.pollingInterval);
            this.pollingInterval = null;
        }
    }

    static async loadNotifications() {
        try {
            const response = await fetch('/api/Notificacoes/ativas?limite=5');
            if (!response.ok) throw new Error('Erro ao carregar notificações');

            const notificacoes = await response.json();
            this.updateBadge(notificacoes.length);
            this.updateDropdown(notificacoes);
        } catch (error) {
            // Silenciosamente falhar para não poluir o console
        }
    }

    static updateBadge(count) {
        const badge = document.getElementById('notification-badge');
        if (badge) {
            badge.textContent = count > 9 ? '9+' : count;
            badge.style.display = count > 0 ? 'inline-block' : 'none';
        }

        // Atualizar título da página
        if (count > 0) {
            document.title = `(${count}) ${document.title.replace(/^\(\d+\)\s/, '')}`;
        } else {
            document.title = document.title.replace(/^\(\d+\)\s/, '');
        }
    }

    static updateDropdown(notificacoes) {
        const dropdown = document.getElementById('notifications-list');
        if (!dropdown) return;

        if (notificacoes.length === 0) {
            dropdown.innerHTML = `
                <div class="dropdown-item text-center text-muted py-4">
                    <i class="fas fa-bell-slash fa-2x mb-2 d-block"></i>
                    <small>Nenhuma notificação</small>
                </div>
            `;
            return;
        }

        dropdown.innerHTML = notificacoes.map(n => this.createNotificationItem(n)).join('');
    }

    static createNotificationItem(notif) {
        const iconMap = {
            'danger': 'exclamation-triangle text-danger',
            'warning': 'exclamation-circle text-warning',
            'info': 'info-circle text-info',
            'success': 'check-circle text-success'
        };

        const icon = iconMap[notif.tipo] || iconMap['info'];

        return `
            <div class="notification-item" data-id="${notif.id}">
                <div class="d-flex align-items-start">
                    <div class="flex-shrink-0 me-3">
                        <i class="fas fa-${icon} fa-lg"></i>
                    </div>
                    <div class="flex-grow-1" style="min-width: 0;">
                        <h6 class="mb-1 text-truncate">${notif.titulo}</h6>
                        <p class="mb-2 small text-muted" style="line-height: 1.4;">${notif.mensagem}</p>
                        <div class="d-flex justify-content-between align-items-center flex-wrap gap-2">
                            <small class="text-muted">${notif.tempoDecorrido}</small>
                            ${notif.linkAcao ? `<a href="${notif.linkAcao}" class="btn btn-sm btn-outline-primary">${notif.textoLinkAcao || 'Ver'}</a>` : ''}
                        </div>
                    </div>
                    <button class="btn btn-sm btn-link text-muted p-1 ms-2" data-notification-id="${notif.id}" title="Marcar como lida">
                        <i class="fas fa-check"></i>
                    </button>
                </div>
            </div>
            <div class="dropdown-divider m-0"></div>
        `;
    }

    static async markAsRead(id) {
        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            const headers = { 'Content-Type': 'application/json' };
            if (token) headers['RequestVerificationToken'] = token;

            const response = await fetch(`/api/Notificacoes/${id}/marcar-lida`, {
                method: 'PUT',
                headers: headers
            });

            if (response.ok) {
                await this.loadNotifications();
                this.success('Notificação marcada como lida', 2000);
            } else {
                console.error('Erro ao marcar notificação como lida:', response.status);
            }
        } catch (error) {
            console.error('Erro ao marcar notificação como lida:', error);
        }
    }

    static async markAllAsRead() {
        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            const headers = { 'Content-Type': 'application/json' };
            if (token) headers['RequestVerificationToken'] = token;

            const response = await fetch('/api/Notificacoes/marcar-todas-lidas', {
                method: 'PUT',
                headers: headers
            });

            if (response.ok) {
                await this.loadNotifications();
                this.success('Todas as notificações foram marcadas como lidas', 3000);
            } else {
                console.error('Erro ao marcar todas como lidas:', response.status);
            }
        } catch (error) {
            console.error('Erro ao marcar todas como lidas:', error);
        }
    }

    static setupBadgeClick() {
        const markAllBtn = document.getElementById('mark-all-read-btn');
        if (markAllBtn) {
            markAllBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.markAllAsRead();
            });
        }

        // Handler para botão "Ver todas"
        const verTodasBtn = document.getElementById('ver-todas-notificacoes');
        if (verTodasBtn) {
            verTodasBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.showAllNotificationsModal();
            });
        }

        // Event delegation para botões de marcar como lida
        document.addEventListener('click', (e) => {
            const btn = e.target.closest('[data-notification-id]');
            if (btn) {
                e.preventDefault();
                const notificationId = parseInt(btn.dataset.notificationId);
                this.markAsRead(notificationId);
            }
        });
    }

    static async showAllNotificationsModal() {
        try {
            const response = await fetch('/api/Notificacoes/ativas?limite=50');
            if (!response.ok) throw new Error('Erro ao carregar notificações');

            const notificacoes = await response.json();
            this.createNotificationsModal(notificacoes);
        } catch (error) {
            console.error('Erro ao carregar todas as notificações:', error);
            this.error('Erro ao carregar notificações');
        }
    }

    static createNotificationsModal(notificacoes) {
        // Remover modal existente se houver
        const existingModal = document.getElementById('allNotificationsModal');
        if (existingModal) existingModal.remove();

        const modalHtml = `
            <div class="modal fade" id="allNotificationsModal" tabindex="-1" aria-labelledby="allNotificationsModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="allNotificationsModalLabel">
                                <i class="fas fa-bell me-2"></i>Todas as Notificações
                            </h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
                        </div>
                        <div class="modal-body p-0">
                            ${notificacoes.length === 0 ? `
                                <div class="text-center py-5">
                                    <i class="fas fa-bell-slash fa-3x text-muted mb-3"></i>
                                    <h6 class="text-muted">Nenhuma notificação</h6>
                                    <p class="text-muted small">Você está em dia com tudo!</p>
                                </div>
                            ` : `
                                <div class="list-group list-group-flush">
                                    ${notificacoes.map(n => this.createModalNotificationItem(n)).join('')}
                                </div>
                            `}
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Fechar</button>
                            ${notificacoes.length > 0 ? `
                                <button type="button" class="btn btn-primary" id="modal-mark-all-read">
                                    <i class="fas fa-check-double me-2"></i>Marcar todas como lidas
                                </button>
                            ` : ''}
                        </div>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', modalHtml);

        const modal = new bootstrap.Modal(document.getElementById('allNotificationsModal'));
        modal.show();

        // Handler para marcar todas como lidas no modal
        const modalMarkAllBtn = document.getElementById('modal-mark-all-read');
        if (modalMarkAllBtn) {
            modalMarkAllBtn.addEventListener('click', async () => {
                await this.markAllAsRead();
                modal.hide();
            });
        }

        // Remover modal do DOM ao fechar
        document.getElementById('allNotificationsModal').addEventListener('hidden.bs.modal', function () {
            this.remove();
        });
    }

    static createModalNotificationItem(notif) {
        const iconMap = {
            'danger': 'exclamation-triangle text-danger',
            'warning': 'exclamation-circle text-warning',
            'info': 'info-circle text-info',
            'success': 'check-circle text-success'
        };

        const icon = iconMap[notif.tipo] || iconMap['info'];
        const bgClass = {
            'danger': 'bg-danger-subtle',
            'warning': 'bg-warning-subtle',
            'info': 'bg-info-subtle',
            'success': 'bg-success-subtle'
        };

        return `
            <div class="list-group-item ${bgClass[notif.tipo] || ''}" data-id="${notif.id}">
                <div class="d-flex align-items-start">
                    <div class="flex-shrink-0 me-3">
                        <i class="fas fa-${icon} fa-2x"></i>
                    </div>
                    <div class="flex-grow-1">
                        <div class="d-flex justify-content-between align-items-start mb-2">
                            <h6 class="mb-0 fw-bold">${notif.titulo}</h6>
                            <small class="text-muted ms-2">${notif.tempoDecorrido}</small>
                        </div>
                        <p class="mb-2">${notif.mensagem}</p>
                        <div class="d-flex justify-content-between align-items-center">
                            <div>
                                ${notif.categoria ? `<span class="badge bg-secondary">${notif.categoria}</span>` : ''}
                            </div>
                            <div>
                                ${notif.linkAcao ? `<a href="${notif.linkAcao}" class="btn btn-sm btn-primary me-2">${notif.textoLinkAcao || 'Ver Detalhes'}</a>` : ''}
                                <button class="btn btn-sm btn-outline-secondary" data-notification-id="${notif.id}" title="Marcar como lida">
                                    <i class="fas fa-check"></i> Marcar como lida
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }
}

// Event listener para pausar polling quando página não está visível
document.addEventListener('visibilitychange', () => {
    if (document.hidden) {
        NotificationSystem.stopPolling();
    } else {
        NotificationSystem.startPolling();
    }
});

// Expor globalmente
window.NotificationSystem = NotificationSystem;

/* ===== LOADING OVERLAY SYSTEM - CORRIGIDO ===== */
class LoadingOverlaySystem {
    static activeOverlays = new Set();

    static show(target = 'body', options = {}) {
        const targetElement = typeof target === 'string' ? document.querySelector(target) : target;
        if (!targetElement || this.activeOverlays.has(targetElement)) return;

        const overlay = document.createElement('div');
        overlay.className = 'loading-overlay-modern';
        overlay.style.cssText = `
            position: ${targetElement === document.body ? 'fixed' : 'absolute'};
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(255, 255, 255, 0.9);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9998;
            backdrop-filter: blur(2px);
        `;

        overlay.innerHTML = `
            <div class="d-flex flex-column align-items-center">
                <div class="spinner-border text-primary mb-2" style="width: 3rem; height: 3rem;" role="status">
                    <span class="visually-hidden">Carregando...</span>
                </div>
                <div class="text-primary fw-semibold">${options.message || 'Processando...'}</div>
            </div>
        `;

        if (targetElement !== document.body) {
            targetElement.style.position = 'relative';
        }

        targetElement.appendChild(overlay);
        this.activeOverlays.add(targetElement);

        // Auto-remove após 10 segundos como fallback
        setTimeout(() => this.hide(targetElement), 10000);

        return overlay;
    }

    static hide(target = 'body') {
        const targetElement = typeof target === 'string' ? document.querySelector(target) : target;
        if (!targetElement) return;

        const overlay = targetElement.querySelector('.loading-overlay-modern');
        if (overlay) {
            overlay.style.opacity = '0';
            setTimeout(() => {
                overlay.remove();
                this.activeOverlays.delete(targetElement);
            }, 200);
        }
    }

    static hideAll() {
        this.activeOverlays.forEach(target => this.hide(target));
        this.activeOverlays.clear();
    }
}

/* ===== DATETIME SYSTEM ===== */
class DateTimeSystem {
    static obterDataHoraBrasil(offsetDias = 0) {
        const agora = new Date();
        if (offsetDias !== 0) {
            agora.setDate(agora.getDate() + offsetDias);
        }
        return agora;
    }

    static formatarParaDateTimeLocal(data) {
        return data.toISOString().slice(0, 16);
    }

    static formatarDataBrasil(data) {
        return data.toLocaleDateString('pt-BR');
    }

    static formatarDataHoraBrasil(data) {
        return data.toLocaleString('pt-BR');
    }

    static inicializarCamposDateTimeBrasil() {
        const configuracoes = {
            'DataRetirada': { offset: 0, hora: 9, preencherAutomatico: true },
            'DataDevolucao': { offset: 0, hora: 18, preencherAutomatico: false },
            'DataViagem': { offset: 7, hora: 9, preencherAutomatico: true },
            'DataInicio': { offset: 0, hora: 9, preencherAutomatico: true },
            'DataFim': { offset: 1, hora: 18, preencherAutomatico: true }
        };

        Object.keys(configuracoes).forEach(nomeCampo => {
            const campo = document.querySelector(`input[name="${nomeCampo}"]`);
            const config = configuracoes[nomeCampo];

            if (campo && !campo.value && config.preencherAutomatico) {
                const data = this.obterDataHoraBrasil(config.offset);
                data.setHours(config.hora, 0, 0, 0);
                campo.value = this.formatarParaDateTimeLocal(data);
            } else if (campo && nomeCampo === 'DataDevolucao') {
                campo.placeholder = 'Selecione a data de devolução';
                const campoRetirada = document.querySelector('input[name="DataRetirada"]');
                if (campoRetirada?.value) {
                    const dataMinima = new Date(campoRetirada.value);
                    dataMinima.setHours(dataMinima.getHours() + 1);
                    campo.min = this.formatarParaDateTimeLocal(dataMinima);
                }
            }
        });

        const outrosCampos = document.querySelectorAll(
            'input[type="datetime-local"]:not([name="DataRetirada"]):not([name="DataDevolucao"]):not([name="DataViagem"]):not([name="DataInicio"]):not([name="DataFim"])'
        );

        outrosCampos.forEach(campo => {
            if (!campo.value) {
                const data = this.obterDataHoraBrasil();
                data.setHours(9, 0, 0, 0);
                campo.value = this.formatarParaDateTimeLocal(data);
            }
        });

        this.configurarEventListenersData();
    }

    static configurarEventListenersData() {
        const campoRetirada = document.querySelector('input[name="DataRetirada"]');
        const campoDevolucao = document.querySelector('input[name="DataDevolucao"]');

        if (campoRetirada) {
            campoRetirada.addEventListener('change', function () {
                const dataRetirada = new Date(this.value);
                if (campoDevolucao) {
                    const dataMinima = new Date(dataRetirada.getTime() + (60 * 60 * 1000));
                    campoDevolucao.min = DateTimeSystem.formatarParaDateTimeLocal(dataMinima);
                    if (campoDevolucao.value && new Date(campoDevolucao.value) <= dataRetirada) {
                        campoDevolucao.value = '';
                        DateTimeSystem.mostrarAviso('Selecione uma data de devolução posterior à retirada');
                    }
                }
            });
        }

        if (campoDevolucao) {
            campoDevolucao.addEventListener('change', function () {
                if (campoRetirada?.value && this.value) {
                    const dataRetirada = new Date(campoRetirada.value);
                    const dataDevolucao = new Date(this.value);
                    if (dataDevolucao <= dataRetirada) {
                        this.setCustomValidity('A data de devolução deve ser posterior à data de retirada.');
                        this.classList.add('is-invalid');
                        return;
                    }
                    this.setCustomValidity('');
                    this.classList.remove('is-invalid');
                    this.classList.add('is-valid');
                }
            });
        }
    }

    static mostrarAviso(mensagem) {
        NotificationSystem.warning(mensagem);
    }

    static validarDataFutura(campo) {
        if (!campo.value) return true;
        const dataEscolhida = new Date(campo.value);
        const agora = new Date();
        return dataEscolhida >= agora;
    }

    static calcularDiferenca(dataInicio, dataFim) {
        const inicio = new Date(dataInicio);
        const fim = new Date(dataFim);
        const diferencaMs = fim - inicio;
        return Math.ceil(diferencaMs / (1000 * 60 * 60 * 24));
    }

    static formatarPeriodoLocacao(dataInicio, dataFim) {
        const dias = this.calcularDiferenca(dataInicio, dataFim);
        return `${dias} ${dias === 1 ? 'dia' : 'dias'}`;
    }
}

/* ===== FORMATTING SYSTEM - CORRIGIDO ===== */
class FormattingSystem {
    static formatarCPF(input) {
        let value = input.value.replace(/\D/g, '');

        // Preservar posição do cursor
        const cursorStart = input.selectionStart;
        const valorAnterior = input.value;

        if (value.length <= 11) {
            value = value.replace(/(\d{3})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
        }

        input.value = value;

        // Restaurar cursor
        let newCursorPos = cursorStart;
        const lengthDiff = value.length - valorAnterior.length;
        if (lengthDiff !== 0) {
            newCursorPos = cursorStart + lengthDiff;
        }
        newCursorPos = Math.max(0, Math.min(newCursorPos, value.length));

        setTimeout(() => {
            input.setSelectionRange(newCursorPos, newCursorPos);
        }, 0);

        // Validar e aplicar classes
        if (value.length === 14) {
            const isValid = this.validateCPF(value);
            input.classList.toggle('is-valid', isValid);
            input.classList.toggle('is-invalid', !isValid);

            // Mostrar feedback visual
            this.showFieldFeedback(input, isValid ? 'CPF válido' : 'CPF inválido', isValid ? 'success' : 'danger');
        } else {
            input.classList.remove('is-valid', 'is-invalid');
            this.clearFieldFeedback(input);
        }

        // Feedback visual na digitação
        input.style.transform = 'scale(1.02)';
        setTimeout(() => input.style.transform = 'scale(1)', 200);
    }

    static formatarTelefone(input) {
        const cursorStart = input.selectionStart;
        const valorAnterior = input.value;

        let numbers = input.value.replace(/\D/g, '');
        if (!numbers) {
            input.value = '';
            input.classList.remove('is-valid', 'is-invalid');
            this.clearFieldFeedback(input);
            return;
        }

        let formattedValue = '';

        if (numbers.length >= 11) {
            formattedValue = numbers.replace(/(\d{2})(\d{5})(\d{4}).*/, '($1) $2-$3');
        } else if (numbers.length >= 10) {
            formattedValue = numbers.replace(/(\d{2})(\d{4})(\d{4}).*/, '($1) $2-$3');
        } else if (numbers.length >= 6) {
            formattedValue = numbers.replace(/(\d{2})(\d{4})(\d+)/, '($1) $2-$3');
        } else if (numbers.length >= 2) {
            formattedValue = numbers.replace(/(\d{2})(\d+)/, '($1) $2');
        } else {
            formattedValue = numbers;
        }

        let newCursorPos = cursorStart;
        const lengthDiff = formattedValue.length - valorAnterior.length;

        if (lengthDiff > 0) {
            newCursorPos = cursorStart + lengthDiff;
        } else if (lengthDiff < 0) {
            newCursorPos = Math.max(0, cursorStart + lengthDiff);
        }

        newCursorPos = Math.max(0, Math.min(newCursorPos, formattedValue.length));
        input.value = formattedValue;

        setTimeout(() => {
            input.setSelectionRange(newCursorPos, newCursorPos);
        }, 0);

        // Validar telefone
        const isValid = formattedValue.length >= 14;
        input.classList.toggle('is-valid', isValid && formattedValue.length === 15);
        input.classList.toggle('is-invalid', !isValid && formattedValue.length > 6);

        if (formattedValue.length >= 14) {
            this.showFieldFeedback(input,
                isValid && formattedValue.length === 15 ? 'Telefone válido' : 'Telefone inválido',
                isValid && formattedValue.length === 15 ? 'success' : 'danger'
            );
        } else {
            this.clearFieldFeedback(input);
        }

        input.style.borderColor = 'var(--bs-primary, #0d6efd)';
        setTimeout(() => {
            input.style.borderColor = '';
        }, 300);
    }

    static formatarMoeda(input) {
        let value = input.value.replace(/\D/g, '');

        if (value === '') {
            input.value = '';
            input.classList.remove('is-valid', 'is-invalid');
            this.clearFieldFeedback(input);
            return;
        }

        value = (parseInt(value) / 100).toFixed(2);
        value = value.replace('.', ',');
        value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
        input.value = 'R$ ' + value;

        input.style.background = 'linear-gradient(90deg, var(--bg-surface-secondary, #f8f9fa), transparent)';
        setTimeout(() => input.style.background = '', 500);

        // Validação simples para moeda
        const numericValue = parseFloat(value.replace(',', '.').replace(/\./g, ''));
        input.classList.toggle('is-valid', numericValue > 0);
    }

    static formatarPlaca(input) {
        let value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, '');

        if (value.length > 3 && value.length <= 7) {
            value = value.replace(/^([A-Z]{3})([0-9A-Z].*)/, '$1-$2');
        }

        input.value = value;

        const isMercosul = /^[A-Z]{3}-[0-9][A-Z][0-9]{2}$/.test(value);
        const isAntigo = /^[A-Z]{3}-[0-9]{4}$/.test(value);
        const isValid = isMercosul || isAntigo;

        input.classList.toggle('is-valid', isValid);
        input.classList.toggle('is-invalid', !isValid && value.length > 0);

        if (value.length >= 7) {
            this.showFieldFeedback(input,
                isValid ? 'Placa válida' : 'Placa inválida',
                isValid ? 'success' : 'danger'
            );
        } else {
            this.clearFieldFeedback(input);
        }
    }

    static validateCPF(cpf) {
        cpf = cpf.replace(/[^\d]/g, '');
        if (cpf.length !== 11 || /^(\d)\1+$/.test(cpf)) {
            return false;
        }
        let sum = 0;
        for (let i = 0; i < 9; i++) {
            sum += parseInt(cpf[i]) * (10 - i);
        }
        let digit1 = (sum * 10) % 11;
        if (digit1 === 10) digit1 = 0;
        if (parseInt(cpf[9]) !== digit1) return false;
        sum = 0;
        for (let i = 0; i < 10; i++) {
            sum += parseInt(cpf[i]) * (11 - i);
        }
        let digit2 = (sum * 10) % 11;
        if (digit2 === 10) digit2 = 0;
        return parseInt(cpf[10]) === digit2;
    }

    static validatePlaca(placa) {
        placa = placa.replace(/[^A-Z0-9]/g, '');
        const formatoAntigo = /^[A-Z]{3}[0-9]{4}$/.test(placa);
        const formatoMercosul = /^[A-Z]{3}[0-9][A-Z][0-9]{2}$/.test(placa);
        return formatoAntigo || formatoMercosul;
    }

    // Sistema de feedback visual unificado
    static showFieldFeedback(input, message, type) {
        this.clearFieldFeedback(input);

        const feedback = document.createElement('div');
        feedback.className = `field-feedback text-${type} mt-1`;
        feedback.innerHTML = `
            <small class="d-flex align-items-center">
                <i class="fas fa-${type === 'success' ? 'check' : 'times'} me-1"></i>
                ${message}
            </small>
        `;

        input.parentNode.appendChild(feedback);
    }

    static clearFieldFeedback(input) {
        const existingFeedback = input.parentNode.querySelector('.field-feedback');
        if (existingFeedback) {
            existingFeedback.remove();
        }
    }
}

/* ===== MASK SYSTEM CORRIGIDO ===== */
class MaskSystem {
    static initialized = false;
    static activeInputs = new WeakSet();

    static initializeMasks() {
        if (this.initialized) return;

        this.initialized = true;
        this.applyMasksToExistingFields();
        this.setupDynamicFieldObserver();
    }

    static applyMasksToExistingFields() {
        // CPF masks - seletores mais abrangentes
        const cpfSelectors = [
            'input[name*="Cpf"]', 'input[name*="CPF"]',
            'input[id*="cpf"]', 'input[id*="CPF"]',
            'input[data-mask="cpf"]', 'input.CPF-mask',
            'input[placeholder*="CPF"]', 'input[placeholder*="cpf"]'
        ];

        cpfSelectors.forEach(selector => {
            document.querySelectorAll(selector).forEach(input => {
                this.applyCpfMask(input);
            });
        });

        // Telefone masks
        const phoneSelectors = [
            'input[name*="Telefone"]', 'input[name*="TELEFONE"]',
            'input[type="tel"]', 'input[id*="telefone"]',
            'input[id*="phone"]', 'input[data-mask="phone"]',
            'input[data-mask="telefone"]', 'input.phone-mask',
            'input[placeholder*="telefone"]', 'input[placeholder*="Telefone"]',
            'input[placeholder*="phone"]'
        ];

        phoneSelectors.forEach(selector => {
            document.querySelectorAll(selector).forEach(input => {
                this.applyPhoneMask(input);
            });
        });

        // Moeda masks
        const moneySelectors = [
            'input[data-mask="moeda"]', 'input[data-mask="money"]',
            'input.money-mask', 'input[name*="Valor"]',
            'input[id*="valor"]', 'input[name*="Preco"]'
        ];

        moneySelectors.forEach(selector => {
            document.querySelectorAll(selector).forEach(input => {
                this.applyMoneyMask(input);
            });
        });

        // Placa masks
        const plateSelectors = [
            'input[name*="Placa"]', 'input[name*="PLACA"]',
            'input[id*="placa"]', 'input[data-mask="placa"]',
            'input.placa-mask'
        ];

        plateSelectors.forEach(selector => {
            document.querySelectorAll(selector).forEach(input => {
                this.applyPlateMask(input);
            });
        });
    }

    static applyCpfMask(input) {
        if (this.activeInputs.has(input)) return;
        this.activeInputs.add(input);

        // Aplicar máscara em valor existente
        if (input.value?.trim()) {
            FormattingSystem.formatarCPF(input);
        }

        const inputHandler = () => FormattingSystem.formatarCPF(input);
        const pasteHandler = () => setTimeout(() => FormattingSystem.formatarCPF(input), 10);

        input.addEventListener('input', inputHandler);
        input.addEventListener('paste', pasteHandler);
    }

    static applyPhoneMask(input) {
        if (this.activeInputs.has(input)) return;
        this.activeInputs.add(input);

        if (input.value?.trim()) {
            FormattingSystem.formatarTelefone(input);
        }

        const inputHandler = () => FormattingSystem.formatarTelefone(input);
        const pasteHandler = () => setTimeout(() => FormattingSystem.formatarTelefone(input), 10);

        input.addEventListener('input', inputHandler);
        input.addEventListener('paste', pasteHandler);
    }

    static applyMoneyMask(input) {
        if (this.activeInputs.has(input)) return;
        this.activeInputs.add(input);

        const moneyHandler = () => FormattingSystem.formatarMoeda(input);
        input.addEventListener('input', moneyHandler);
    }

    static applyPlateMask(input) {
        if (this.activeInputs.has(input)) return;
        this.activeInputs.add(input);

        const plateHandler = () => FormattingSystem.formatarPlaca(input);
        input.addEventListener('input', plateHandler);
    }

    static setupDynamicFieldObserver() {
        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (node.nodeType === Node.ELEMENT_NODE) {
                        this.checkAndApplyMask(node);
                        const inputs = node.querySelectorAll ? node.querySelectorAll('input') : [];
                        inputs.forEach(input => this.checkAndApplyMask(input));
                    }
                });
            });
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    static checkAndApplyMask(element) {
        if (!element || element.tagName !== 'INPUT' || this.activeInputs.has(element)) return;

        const name = (element.name || '').toLowerCase();
        const id = (element.id || '').toLowerCase();
        const className = element.className || '';
        const type = element.type || '';
        const placeholder = (element.placeholder || '').toLowerCase();
        const dataMask = element.getAttribute('data-mask') || '';

        if (name.includes('cpf') || id.includes('cpf') || className.includes('cpf') ||
            placeholder.includes('cpf') || dataMask === 'cpf') {
            this.applyCpfMask(element);
        }
        else if (name.includes('telefone') || type === 'tel' || id.includes('telefone') ||
            id.includes('phone') || className.includes('phone') ||
            placeholder.includes('telefone') || dataMask === 'phone' || dataMask === 'telefone') {
            this.applyPhoneMask(element);
        }
        else if (name.includes('valor') || name.includes('preco') || id.includes('valor') || className.includes('money') ||
            dataMask === 'moeda' || dataMask === 'money') {
            this.applyMoneyMask(element);
        }
        else if (name.includes('placa') || id.includes('placa') || className.includes('placa') ||
            dataMask === 'placa') {
            this.applyPlateMask(element);
        }
    }
}

/* ===== VALIDATION SYSTEM INTEGRADO E CORRIGIDO ===== */
class ValidationSystem {
    static initialized = false;

    static init() {
        if (this.initialized) return;

        this.setupRealTimeValidation();
        this.setupFormSubmissions();
        this.initialized = true;
    }

    static setupRealTimeValidation() {
        // Validação em tempo real durante digitação
        document.addEventListener('input', (e) => {
            if (e.target.tagName === 'INPUT' || e.target.tagName === 'SELECT' || e.target.tagName === 'TEXTAREA') {
                // Não validar se o campo estiver vazio e não for obrigatório
                if (!e.target.value?.trim() && !e.target.required) {
                    e.target.classList.remove('is-valid', 'is-invalid');
                    this.clearValidationFeedback(e.target);
                    return;
                }

                // Validar apenas se há conteúdo ou é obrigatório
                if (e.target.value?.trim() || e.target.required) {
                    this.validateField(e.target);
                }
            }
        });

        // Validação ao perder foco
        document.addEventListener('blur', (e) => {
            if (e.target.tagName === 'INPUT' || e.target.tagName === 'SELECT' || e.target.tagName === 'TEXTAREA') {
                this.validateField(e.target);
            }
        }, true);
    }

    static validateField(field) {
        if (!field) return true;

        // Limpar validação anterior
        this.clearValidationFeedback(field);

        // Campo vazio e não obrigatório
        if (!field.value?.trim() && !field.required) {
            field.classList.remove('is-valid', 'is-invalid');
            return true;
        }

        // Campo obrigatório vazio
        if (field.required && !field.value?.trim()) {
            field.classList.add('is-invalid');
            field.classList.remove('is-valid');
            this.showValidationFeedback(field, this.getRequiredMessage(field), 'invalid');
            return false;
        }

        let isValid = true;
        let message = '';

        // Validação por tipo de campo
        const fieldName = field.name?.toLowerCase() || '';
        const fieldType = field.type || '';

        if (fieldName.includes('cpf') && field.value) {
            isValid = FormattingSystem.validateCPF(field.value);
            message = isValid ? 'CPF válido' : 'CPF inválido';
        }
        else if (fieldName.includes('placa') && field.value) {
            isValid = FormattingSystem.validatePlaca(field.value);
            message = isValid ? 'Placa válida' : 'Placa inválida';
        }
        else if (fieldType === 'email' && field.value) {
            isValid = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(field.value);
            message = isValid ? 'Email válido' : 'Email inválido';
        }
        else if (fieldName.includes('telefone') && field.value) {
            const numbers = field.value.replace(/\D/g, '');
            isValid = numbers.length === 10 || numbers.length === 11;
            message = isValid ? 'Telefone válido' : 'Telefone deve ter 10 ou 11 dígitos';
        }
        else if (fieldType === 'datetime-local' || fieldType === 'date') {
            // Validação de data futura para campos específicos
            if (fieldName.includes('retirada') || fieldName.includes('viagem') || fieldName.includes('inicio')) {
                isValid = DateTimeSystem.validarDataFutura(field);
                message = isValid ? 'Data válida' : 'Data deve ser hoje ou no futuro';
            }

            // Validação de data de devolução
            if (fieldName.includes('devolucao') || fieldName.includes('fim')) {
                const startField = document.querySelector('input[name*="Retirada"], input[name*="Inicio"]');
                if (startField?.value && field.value) {
                    const startDate = new Date(startField.value);
                    const endDate = new Date(field.value);
                    isValid = endDate > startDate;
                    message = isValid ? 'Data válida' : 'Data deve ser posterior à data inicial';
                }
            }
        }
        else {
            // Validação nativa do HTML5
            isValid = field.checkValidity();
            message = isValid ? 'Campo válido' : field.validationMessage || 'Campo inválido';
        }

        // Aplicar classes visuais
        field.classList.toggle('is-valid', isValid);
        field.classList.toggle('is-invalid', !isValid);

        // Mostrar feedback apenas se houver erro
        if (!isValid) {
            this.showValidationFeedback(field, message, 'invalid');
        }

        return isValid;
    }

    static setupFormSubmissions() {
        document.addEventListener('submit', (e) => {
            // Identificar se é um formulário que precisa de validação
            if (e.target.classList.contains('needs-validation') ||
                e.target.classList.contains('form-cliente')) {

                e.preventDefault();
                e.stopPropagation();

                const inputs = e.target.querySelectorAll('input, select, textarea');
                let isValid = true;
                let firstInvalidField = null;

                inputs.forEach(input => {
                    const fieldIsValid = this.validateField(input);
                    if (!fieldIsValid) {
                        isValid = false;
                        if (!firstInvalidField) {
                            firstInvalidField = input;
                        }
                    }
                });

                e.target.classList.add('was-validated');

                if (isValid) {
                    this.submitFormWithLoading(e.target);
                } else {
                    if (firstInvalidField) {
                        firstInvalidField.scrollIntoView({ behavior: 'smooth', block: 'center' });
                        firstInvalidField.focus();
                    }
                    NotificationSystem.error('Por favor, corrija os campos em vermelho antes de continuar.');
                }
            }
        });
    }

    static submitFormWithLoading(form) {
        // Verificar se é modal de confirmação - NÃO aplicar loading
        if (form.closest('.modal') || form.id === 'formExclusao') {
            form.submit();
            return;
        }

        const submitBtn = form.querySelector('button[type="submit"]');
        if (!submitBtn) {
            form.submit();
            return;
        }

        const originalText = submitBtn.innerHTML;

        // Aplicar efeito visual nos campos (sem desabilitar para não bloquear o envio)
        const inputs = form.querySelectorAll('input, select, textarea');
        inputs.forEach(input => {
            input.style.opacity = '0.6';
            input.style.pointerEvents = 'none';
        });

        // Desabilitar botão e mostrar loading
        submitBtn.disabled = true;
        submitBtn.innerHTML = `
            <span class="spinner-border spinner-border-sm me-2" role="status"></span>
            Carregando...
        `;

        // Delay para UX
        setTimeout(() => {
            form.submit();
        }, 800);
    }

    static showValidationFeedback(field, message, type) {
        this.clearValidationFeedback(field);

        // Usar feedback nativo do Bootstrap primeiro
        let feedback = field.parentNode.querySelector('.invalid-feedback, .valid-feedback');

        if (!feedback) {
            feedback = document.createElement('div');
            feedback.className = type === 'invalid' ? 'invalid-feedback' : 'valid-feedback';

            // Inserir após o campo
            field.parentNode.insertBefore(feedback, field.nextSibling);
        }

        feedback.innerHTML = message;
        feedback.style.display = 'block';
    }

    static clearValidationFeedback(field) {
        const feedbacks = field.parentNode.querySelectorAll('.invalid-feedback, .valid-feedback, .field-feedback');
        feedbacks.forEach(feedback => {
            if (!feedback.getAttribute('asp-validation-for')) {
                feedback.remove();
            }
        });
    }

    static getRequiredMessage(field) {
        const fieldName = field.name || field.id || 'Campo';
        const customMessages = {
            'Nome': 'Nome é obrigatório',
            'Email': 'Email é obrigatório',
            'Telefone': 'Telefone é obrigatório',
            'Cpf': 'CPF é obrigatório',
            'DataNascimento': 'Data de nascimento é obrigatória',
            'Endereco': 'Endereço é obrigatório'
        };

        return customMessages[fieldName] || `${fieldName} é obrigatório`;
    }
}

/* ===== SEARCH AND FILTER FUNCTIONS ===== */
function setupAdvancedSearch(inputSelector, tableSelector) {
    const searchInput = document.querySelector(inputSelector);
    const table = document.querySelector(tableSelector);

    if (!searchInput || !table) return;

    let searchTimeout;

    searchInput.addEventListener('input', function () {
        clearTimeout(searchTimeout);
        const query = this.value.trim().toLowerCase();

        searchTimeout = setTimeout(() => {
            const rows = table.querySelectorAll('tbody tr');
            let visibleCount = 0;

            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                const isMatch = query.length < 2 || text.includes(query);
                row.style.display = isMatch ? '' : 'none';
                if (isMatch) visibleCount++;
            });

            updateSearchResultsCount(table, visibleCount, rows.length);
        }, 300);
    });
}

function updateSearchResultsCount(table, visible, total) {
    let counter = table.parentNode.querySelector('.search-results-counter');
    if (!counter) {
        counter = document.createElement('div');
        counter.className = 'search-results-counter alert alert-info p-2 mt-2';
        table.parentNode.appendChild(counter);
    }
    if (visible === total) {
        counter.style.display = 'none';
    } else {
        counter.style.display = 'block';
        counter.innerHTML = `<i class="fas fa-search me-2"></i>Mostrando ${visible} de ${total} registros`;
    }
}

/* ===== TABLE SORTING FUNCTIONS ===== */
function setupAdvancedTableSort(tableSelector) {
    const table = document.querySelector(tableSelector);
    if (!table) return;

    const headers = table.querySelectorAll('th[data-sort], th.sortable');
    if (headers.length === 0) return;

    let currentSort = { column: null, direction: 'asc' };

    headers.forEach(header => {
        header.style.cursor = 'pointer';
        header.style.userSelect = 'none';

        if (!header.querySelector('.sort-icon')) {
            header.innerHTML += ' <i class="fas fa-sort text-muted sort-icon"></i>';
        }

        header.addEventListener('click', function () {
            const column = this.getAttribute('data-sort') || this.textContent.trim().toLowerCase();
            const dataType = this.getAttribute('data-type') || 'string';

            if (currentSort.column === column) {
                currentSort.direction = currentSort.direction === 'asc' ? 'desc' : 'asc';
            } else {
                currentSort.direction = 'asc';
            }
            currentSort.column = column;

            headers.forEach(h => {
                const icon = h.querySelector('.sort-icon');
                if (icon) icon.className = 'fas fa-sort text-muted sort-icon';
            });

            const icon = this.querySelector('.sort-icon');
            if (icon) {
                icon.className = `fas fa-sort-${currentSort.direction === 'asc' ? 'up' : 'down'} text-primary sort-icon`;
            }

            sortTable(table, column, currentSort.direction, dataType);
        });
    });
}

function sortTable(table, column, direction, dataType) {
    const tbody = table.querySelector('tbody');
    if (!tbody) return;

    const rows = Array.from(tbody.querySelectorAll('tr'));

    rows.sort((a, b) => {
        let aVal = getValueFromRow(a, column, table);
        let bVal = getValueFromRow(b, column, table);

        switch (dataType) {
            case 'number':
                aVal = parseFloat(aVal.replace(/[^\d.-]/g, '')) || 0;
                bVal = parseFloat(bVal.replace(/[^\d.-]/g, '')) || 0;
                break;
            case 'date':
                aVal = new Date(aVal);
                bVal = new Date(bVal);
                break;
            case 'currency':
                aVal = parseFloat(aVal.replace(/[^\d,]/g, '').replace(',', '.')) || 0;
                bVal = parseFloat(bVal.replace(/[^\d,]/g, '').replace(',', '.')) || 0;
                break;
            default:
                aVal = aVal.toLowerCase();
                bVal = bVal.toLowerCase();
        }

        let result = 0;
        if (aVal < bVal) result = -1;
        if (aVal > bVal) result = 1;
        return direction === 'desc' ? -result : result;
    });

    tbody.style.opacity = '0.7';
    setTimeout(() => {
        tbody.innerHTML = '';
        rows.forEach(row => tbody.appendChild(row));
        tbody.style.opacity = '1';
    }, 200);
}

function getValueFromRow(row, column, table) {
    let cell = row.querySelector(`[data-sort-value]`);
    if (cell && cell.getAttribute('data-sort-value') === column) {
        return cell.textContent.trim();
    }

    const columnIndex = getColumnIndex(table, column);
    cell = row.querySelector(`td:nth-child(${columnIndex})`);
    return cell ? cell.textContent.trim() : '';
}

function getColumnIndex(table, columnName) {
    const headers = table.querySelectorAll('th');
    for (let i = 0; i < headers.length; i++) {
        const headerText = headers[i].textContent.trim().toLowerCase();
        const dataSort = headers[i].getAttribute('data-sort');
        if (dataSort === columnName || headerText === columnName) {
            return i + 1;
        }
    }
    return 1;
}

/* ===== FORM VALIDATION SETUP FUNCTION ===== */
function setupAdvancedFormValidation() {
    // Esta função agora apenas chama o sistema integrado
    if (!ValidationSystem.initialized) {
        ValidationSystem.init();
    }
}

/* ===== HELPER FUNCTIONS ===== */
function initializeTooltips() {
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl, {
                animation: true,
                delay: { show: 500, hide: 100 },
                html: true,
                placement: 'auto'
            });
        });
    }
}

function setupDeleteConfirmations() {
    document.querySelectorAll('.btn-delete, .delete-confirm').forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();
            const itemName = this.getAttribute('data-item-name') || 'este item';
            const confirmed = confirm(`Tem certeza que deseja excluir ${itemName}?\nEsta ação não pode ser desfeita.`);
            if (confirmed) {
                if (this.tagName === 'A') {
                    window.location.href = this.href;
                } else if (this.form) {
                    this.form.submit();
                }
            }
        });
    });
}

function initializeKeyboardShortcuts() {
    document.addEventListener('keydown', function (e) {
        if (e.altKey && e.key === 's') {
            e.preventDefault();
            const searchInput = document.querySelector('#searchInput, input[type="search"]');
            if (searchInput) {
                searchInput.focus();
            }
        }
        if (e.altKey && e.key === 'n') {
            e.preventDefault();
            const createButton = document.querySelector('a[href*="/Create"], .btn-create');
            if (createButton) {
                createButton.click();
            }
        }
    });
}

/* ===== PERFORMANCE MONITORING ===== */
class PerformanceMonitor {
    static init() {
        window.addEventListener('load', () => {
            const loadTime = performance.now();
            // Monitoramento silencioso de performance
        });
    }
}

/* ===== INITIALIZATION CONSOLIDADO ===== */
document.addEventListener('DOMContentLoaded', function () {
    // 1. Sistemas principais
    window.sidebarManager = new SidebarManager();
    window.themeManager = new ThemeManager();
    PerformanceMonitor.init();

    // 1.5. Sistema de notificações (carregar cedo)
    setTimeout(() => {
        NotificationSystem.initialize();
    }, 10);

    // 2. Sistema de máscaras (prioridade alta)
    setTimeout(() => {
        MaskSystem.initializeMasks();
    }, 50);

    // 3. Sistema de validação integrado
    setTimeout(() => {
        ValidationSystem.init();
    }, 100);

    // 4. Sistema de datas
    setTimeout(() => {
        DateTimeSystem.inicializarCamposDateTimeBrasil();
    }, 150);

    // 5. Recursos auxiliares
    setTimeout(() => {
        setupAdvancedSearch('#searchInput, input[type="search"]', 'table');
        setupAdvancedTableSort('table.sortable, table[data-sortable]');
        initializeTooltips();
        setupDeleteConfirmations();
        initializeKeyboardShortcuts();
    }, 200);

    // 6. Configurações específicas de data
    setTimeout(() => {
        const camposDataFutura = document.querySelectorAll('input[name="DataRetirada"], input[name="DataViagem"], input[name="DataInicio"]');
        camposDataFutura.forEach(campo => {
            campo.addEventListener('change', function () {
                if (!DateTimeSystem.validarDataFutura(this)) {
                    this.setCustomValidity('A data deve ser hoje ou no futuro');
                    this.classList.add('is-invalid');
                } else {
                    this.setCustomValidity('');
                    this.classList.remove('is-invalid');
                    this.classList.add('is-valid');
                }
            });
        });
    }, 250);
});

/* ===== GLOBAL EXPORTS ===== */
window.RentalTourismSystem = {
    SidebarManager,
    ThemeManager,
    NotificationSystem,
    LoadingOverlaySystem,
    DateTimeSystem,
    FormattingSystem,
    MaskSystem,
    ValidationSystem,
    PerformanceMonitor
};

// Compatibilidade com código existente
window.formatarCPF = (input) => FormattingSystem.formatarCPF(input);
window.formatarTelefone = (input) => FormattingSystem.formatarTelefone(input);
window.formatarMoeda = (input) => FormattingSystem.formatarMoeda(input);
window.formatarPlaca = (input) => FormattingSystem.formatarPlaca(input);
window.obterDataHoraBrasil = (offset) => DateTimeSystem.obterDataHoraBrasil(offset);
window.formatarParaDateTimeLocal = (data) => DateTimeSystem.formatarParaDateTimeLocal(data);
window.mostrarNotificacao = (msg, type) => NotificationSystem.show(msg, type);
window.showLoadingOverlay = (target, options) => LoadingOverlaySystem.show(target, options);
window.hideLoadingOverlay = (target) => LoadingOverlaySystem.hide(target);

// Funções auxiliares globais
window.initializeTooltips = initializeTooltips;
window.setupAdvancedFormValidation = setupAdvancedFormValidation;
window.setupAdvancedSearch = setupAdvancedSearch;
window.setupAdvancedTableSort = setupAdvancedTableSort;