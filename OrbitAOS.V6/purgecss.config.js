/**
 * PurgeCSS Configuration
 * Rule: cz-css-1005 - Unused CSS in Container Images
 *
 * This configuration removes unused CSS rules from Bootstrap files
 * during the container image build process to reduce image size
 * and improve Kubernetes deployment performance.
 *
 * Unused selectors identified:
 *   - .fw-bold, .fw-bolder (bootstrap-utilities.css lines 1220, 1224)
 *   - .fw-bold, .fw-bolder (bootstrap-utilities.rtl.css lines 1220, 1224)
 *   - .placeholder, .placeholder-xs, .placeholder-sm, .placeholder-lg,
 *     .placeholder-glow, .placeholder-wave (bootstrap.css lines 6299, 6312,
 *     6316, 6320, 6324, 6340, 7575, 7579)
 *   - .placeholder, .placeholder-xs, .placeholder-sm, .placeholder-lg,
 *     .placeholder-glow, .placeholder-wave (bootstrap.rtl.css lines 6284,
 *     6297, 6301, 6305, 6309, 6325, 7560, 7564)
 */

module.exports = {
  // Content files to scan for used CSS selectors
  content: [
    './Views/**/*.cshtml',
    './Areas/**/*.cshtml',
    './wwwroot/js/**/*.js',
    './Pages/**/*.cshtml',
  ],

  // CSS files to purge
  css: [
    './wwwroot/lib/bootstrap/dist/css/bootstrap.css',
    './wwwroot/lib/bootstrap/dist/css/bootstrap.rtl.css',
    './wwwroot/lib/bootstrap/dist/css/bootstrap-utilities.css',
    './wwwroot/lib/bootstrap/dist/css/bootstrap-utilities.rtl.css',
    './wwwroot/css/site.css',
  ],

  // Output directory for purged CSS files
  output: './wwwroot/lib/bootstrap/dist/css/',

  // Safelist: selectors that must be kept even if not found in content
  // Add any dynamically-generated class names here
  safelist: {
    standard: [
      // Bootstrap core layout and navigation classes always needed
      /^navbar/,
      /^nav-/,
      /^container/,
      /^row/,
      /^col-/,
      /^d-/,
      /^flex-/,
      /^justify-/,
      /^align-/,
      /^text-/,
      /^bg-/,
      /^border/,
      /^btn/,
      /^form-/,
      /^input-/,
      /^modal/,
      /^dropdown/,
      /^collapse/,
      /^show/,
      /^fade/,
      /^active/,
      /^disabled/,
      /^is-/,
      /^was-/,
      /^mb-/,
      /^mt-/,
      /^ms-/,
      /^me-/,
      /^p-/,
      /^pb-/,
      /^pt-/,
      /^ps-/,
      /^pe-/,
      /^m-/,
      /^g-/,
      /^gap-/,
      /^w-/,
      /^h-/,
      /^fs-/,
      /^fw-light/,
      /^fw-normal/,
      /^fw-semibold/,
      /^lh-/,
      /^list-/,
      /^table/,
      /^badge/,
      /^alert/,
      /^card/,
      /^accordion/,
      /^breadcrumb/,
      /^pagination/,
      /^progress/,
      /^spinner/,
      /^toast/,
      /^tooltip/,
      /^popover/,
      /^offcanvas/,
      /^visually-/,
      /^sr-/,
      /^overflow-/,
      /^position-/,
      /^top-/,
      /^bottom-/,
      /^start-/,
      /^end-/,
      /^translate-/,
      /^shadow/,
      /^rounded/,
      /^ratio/,
      /^object-/,
      /^float-/,
      /^clearfix/,
      /^vr/,
      /^link-/,
      /^stretched-/,
    ],
    // Unused component selectors flagged by cz-css-1005 are NOT safelisted
    // so PurgeCSS will remove them if not found in content:
    //   .fw-bold, .fw-bolder
    //   .placeholder, .placeholder-xs, .placeholder-sm, .placeholder-lg
    //   .placeholder-glow, .placeholder-wave
  },

  // Variables: keep CSS custom properties
  variables: true,

  // Keyframes: remove unused @keyframes
  keyframes: true,

  // Font faces: remove unused @font-face
  fontFace: false,

  // Rejected CSS: log removed selectors for audit
  rejected: true,
  rejectedCss: true,
};
