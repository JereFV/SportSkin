/**
 * SportSkin — Configuración e integración de i18next
 * ===================================================
 * Este archivo inicializa i18next con:
 *   - HttpBackend: carga archivos JSON desde /Resources/{lng}/{ns}.json
 *   - BrowserLanguageDetector: detecta el idioma del usuario automáticamente
 *
 * Expone el objeto global `sportSkinI18n` con helpers para la app.
 *
 * Uso en vistas Razor:
 *   <span data-i18n="nav.home"></span>
 *   <input data-i18n-placeholder="filters.search" />
 */

const sportSkinI18n = (() => {

    // ─── Módulos (namespaces) de traducción ───────────────────────────────────
    // Cada módulo corresponde a un archivo JSON en /Resources/{lng}/{ns}.json
    const NAMESPACES = ['common', 'shirts', 'auctions', 'users', 'home'];
    const DEFAULT_NS = 'common';
    const SUPPORTED_LANGUAGES = ['es', 'en'];

    // ─── Configuración de i18next ─────────────────────────────────────────────

    const i18nConfig = {
        // Namespaces a cargar
        ns: NAMESPACES,
        defaultNS: DEFAULT_NS,

        // Idioma de respaldo si una clave no existe en el idioma activo
        fallbackLng: 'es',

        // Separadores de clave y namespace
        // keySeparator '.' permite resolver objetos anidados: "popular.viewAll"
        // nsSeparator ':' lo manejamos manualmente en el PATCH (_resolve)
        // Se deja 'false' para que el bundle no interfiera con el parseo manual
        keySeparator: '.',
        nsSeparator: false,

        // Idiomas soportados — evita que el detector guarde "es-CR" o "en-US"
        supportedLngs: SUPPORTED_LANGUAGES,

        // Cargar solo el código de idioma base: 'es' en vez de 'es-CR'
        load: 'languageOnly',

        // i18next debug en consola (desactivar en producción)
        debug: false,

        // Interpolación: mantener el formato {{variable}} de i18next
        interpolation: {
            // Razor ya escapa HTML, no necesitamos doble escape
            escapeValue: false,
            prefix: '{{',
            suffix: '}}',
        },

        // ── HttpBackend: ruta de los archivos JSON ────────────────────────────
        backend: {
            loadPath: '/Resources/{{lng}}/{{ns}}.json',
            // Cache busting para desarrollo: agregar timestamp
            queryStringParams: {},
            // Reintentar si falla la carga
            allowMultiLoading: false,
            // Parse manual si se necesita preprocesamiento
            parse: JSON.parse,
        },

        // Persistencia de idioma: manejada manualmente en init() via localStorage
    };

    // ─── Inicialización ───────────────────────────────────────────────────────

    /**
     * Inicializa i18next, aplica traducciones al DOM y emite 'sportSkin:i18nReady'.
     * Se llama automáticamente en DOMContentLoaded.
     */
    async function init() {
        try {
            // Leer idioma guardado por el usuario. Si no existe o no es válido,
            // usar el idioma del navegador truncado a 2 caracteres, o 'es' como fallback.
            const saved = localStorage.getItem('i18nextLng');
            const browserLng = (navigator.language || 'es').substring(0, 2);
            const lng = SUPPORTED_LANGUAGES.includes(saved)
                ? saved
                : SUPPORTED_LANGUAGES.includes(browserLng)
                    ? browserLng
                    : 'es';

            // Normalizar: persistir el valor limpio para la próxima carga de página
            localStorage.setItem('i18nextLng', lng);

            await i18next
                .use(i18nextHttpBackend)
                .init({ ...i18nConfig, lng });

            // Esperar a que todos los namespaces estén cargados antes de aplicar al DOM.
            // Con HttpBackend, init() puede resolver antes de que todos los JSON lleguen.
            await Promise.all(
                NAMESPACES.map(ns => i18next.loadNamespaces(ns))
            );

            // Aplicar traducciones al DOM inicial
            applyToDom();

            // Actualizar el atributo lang del <html> para accesibilidad y SEO
            document.documentElement.setAttribute('lang', lng);

            // Notificar al resto de la app
            document.dispatchEvent(new CustomEvent('sportSkin:i18nReady', {
                detail: { language: lng }
            }));

            if (i18nConfig.debug) {
                console.info('[i18n] Inicializado. Idioma:', lng);
            }

        } catch (err) {
            console.error('[i18n] Error de inicialización:', err);
        }
    }

    // ─── Aplicación al DOM ────────────────────────────────────────────────────

    /**
     * Recorre el DOM (o un sub-árbol) y aplica las traducciones.
     *
     * Atributos soportados:
     *   data-i18n="ns:key"              → innerText  (ns: es opcional, usa defaultNS)
     *   data-i18n-placeholder="ns:key"  → placeholder
     *   data-i18n-title="ns:key"        → title
     *   data-i18n-aria-label="ns:key"   → aria-label
     *   data-i18n-html="ns:key"         → innerHTML (usar con precaución)
     *
     * Interpolación: data-i18n-options='{"count": 3}' para pasar parámetros.
     */
    function applyToDom(root) {
        const container = root || document;

        // ── innerText ──────────────────────────────────────────────────────────
        container.querySelectorAll('[data-i18n]').forEach(el => {
            const key = el.getAttribute('data-i18n');
            const options = _getOptions(el);
            el.textContent = i18next.t(key, options);
        });

        // ── placeholder ────────────────────────────────────────────────────────
        container.querySelectorAll('[data-i18n-placeholder]').forEach(el => {
            const key = el.getAttribute('data-i18n-placeholder');
            const options = _getOptions(el);
            el.setAttribute('placeholder', i18next.t(key, options));
        });

        // ── title (tooltip) ────────────────────────────────────────────────────
        container.querySelectorAll('[data-i18n-title]').forEach(el => {
            const key = el.getAttribute('data-i18n-title');
            const options = _getOptions(el);
            el.setAttribute('title', i18next.t(key, options));
        });

        // ── aria-label (accesibilidad) ─────────────────────────────────────────
        container.querySelectorAll('[data-i18n-aria-label]').forEach(el => {
            const key = el.getAttribute('data-i18n-aria-label');
            const options = _getOptions(el);
            el.setAttribute('aria-label', i18next.t(key, options));
        });

        // ── innerHTML (solo para contenido confiable) ─────────────────────────
        container.querySelectorAll('[data-i18n-html]').forEach(el => {
            const key = el.getAttribute('data-i18n-html');
            const options = _getOptions(el);
            el.innerHTML = i18next.t(key, options);
        });
    }

    /**
     * Lee las opciones de interpolación del atributo data-i18n-options (JSON).
     * Ejemplo: <span data-i18n="bids.count" data-i18n-options='{"count":5}'></span>
     */
    function _getOptions(el) {
        const raw = el.getAttribute('data-i18n-options');
        if (!raw) return {};
        try { return JSON.parse(raw); } catch (e) { return {}; }
    }

    // ─── Cambio de idioma ─────────────────────────────────────────────────────

    /**
     * Cambia el idioma activo, recarga los recursos si son necesarios,
     * aplica las traducciones al DOM, y persiste la elección.
     *
     * @param {string} lng  Código de idioma ('es' | 'en')
     * @returns {Promise<void>}
     */
    async function changeLanguage(lng) {
        if (!SUPPORTED_LANGUAGES.includes(lng)) {
            console.warn('[i18n] Idioma no soportado:', lng);
            return;
        }

        try {
            await i18next.changeLanguage(lng);

            // Persistir manualmente para garantizar que la selección
            // sobreviva la navegación entre páginas
            localStorage.setItem('i18nextLng', lng);

            applyToDom();
            document.documentElement.setAttribute('lang', lng);

            document.dispatchEvent(new CustomEvent('sportSkin:languageChanged', {
                detail: { language: lng }
            }));

        } catch (err) {
            console.error('[i18n] Error al cambiar idioma:', err);
        }
    }

    /**
     * Shortcut para traducir desde código JS directamente.
     * Equivale a i18next.t(key, options).
     */
    function t(key, options) {
        return i18next.t(key, options);
    }

    /**
     * Retorna el idioma activo.
     */
    function getCurrentLanguage() {
        return i18next.language || 'es';
    }

    // ─── Auto-init ────────────────────────────────────────────────────────────
    document.addEventListener('DOMContentLoaded', init);

    // ─── API pública ──────────────────────────────────────────────────────────
    return {
        init,
        t,
        changeLanguage,
        getCurrentLanguage,
        applyToDom,
        SUPPORTED_LANGUAGES,
        NAMESPACES,
    };

})();// PATCH: sobrescribir applyToDom para parsear "ns:key" manualmente
(function () {
    /**
     * Pluralización manual — el bundle UMD no incluye plural resolver.
     * Busca key_one / key_other en el resource bundle directamente.
     * Soporta español e inglés (regla: count === 1 → _one, resto → _other).
     */
    function _plural(ns, key, count) {
        const lng = i18next.language || 'es';
        const bundle = i18next.getResourceBundle(lng, ns) || {};

        // Navegar objetos anidados: "bids.count" → bundle.bids.count_one
        const parts = key.split('.');
        let obj = bundle;
        for (let i = 0; i < parts.length - 1; i++) {
            obj = obj[parts[i]];
            if (!obj) return undefined;
        }
        const baseKey = parts[parts.length - 1];
        const suffix = (count === 1) ? '_one' : '_other';
        return obj[baseKey + suffix];
    }

    function _resolve(rawKey, options) {
        options = options || {};
        var ns, key;

        if (rawKey && rawKey.includes(':')) {
            const idx = rawKey.indexOf(':');
            ns = rawKey.substring(0, idx);
            key = rawKey.substring(idx + 1);
        } else {
            ns = i18next.options.defaultNS || 'common';
            key = rawKey;
        }

        // Si viene count en options, resolver pluralización manualmente
        var result;
        if (options.count !== undefined) {
            result = _plural(ns, key, options.count);
        }

        // Fallback: dejar que i18next intente resolverlo
        if (result === undefined) {
            result = i18next.t(key, Object.assign({ ns: ns }, options));
        }

        // Si i18next devolvió la key literal, no sobreescribir el fallback HTML
        if (result === rawKey || result === key) {
            return undefined;
        }
        return result;
    }

    function _opts(el) {
        const raw = el.getAttribute('data-i18n-options');
        if (!raw) return {};
        try { return JSON.parse(raw); } catch (e) { return {}; }
    }

    sportSkinI18n.applyToDom = function (root) {
        const c = root || document;

        c.querySelectorAll('[data-i18n]').forEach(el => {
            const val = _resolve(el.getAttribute('data-i18n'), _opts(el));
            if (val !== undefined) el.textContent = val;
        });
        c.querySelectorAll('[data-i18n-placeholder]').forEach(el => {
            const val = _resolve(el.getAttribute('data-i18n-placeholder'), _opts(el));
            if (val !== undefined) el.setAttribute('placeholder', val);
        });
        c.querySelectorAll('[data-i18n-title]').forEach(el => {
            const val = _resolve(el.getAttribute('data-i18n-title'), _opts(el));
            if (val !== undefined) el.setAttribute('title', val);
        });
        c.querySelectorAll('[data-i18n-aria-label]').forEach(el => {
            const val = _resolve(el.getAttribute('data-i18n-aria-label'), _opts(el));
            if (val !== undefined) el.setAttribute('aria-label', val);
        });
        c.querySelectorAll('[data-i18n-html]').forEach(el => {
            const val = _resolve(el.getAttribute('data-i18n-html'), _opts(el));
            if (val !== undefined) el.innerHTML = val;
        });
    };

    // Re-lanzar applyToDom cuando i18n esté listo
    document.addEventListener('sportSkin:i18nReady', function () {
        sportSkinI18n.applyToDom();
    });
    document.addEventListener('sportSkin:languageChanged', function () {
        sportSkinI18n.applyToDom();
    });
})()