# Quick Reference Guide - UI/UX Design System

This guide provides quick reference for maintaining consistent UI/UX across the UniGuesserRun application.

## Spacing Scale

Use these values for margins, padding, and gaps:

```scss
// Tight spacing
4px   (0.25rem)  - Small gaps, hints
8px   (0.5rem)   - Medium gaps, internal spacing
12px  (0.75rem)  - Button padding, moderate spacing
16px  (1rem)     - Standard spacing, container padding
20px  (1.25rem)  - Larger spacing, section margins
24px  (1.5rem)   - Major spacing, card padding
32px  (2rem)     - Large spacing, section breaks
```

## Border Radius

```scss
8px   - Small elements (inputs, small buttons)
12px  - Medium elements (cards, images)
16px  - Large elements (major containers, buttons)
100px - Pill-shaped elements (menu buttons)
```

## Shadows

```scss
// Small shadow - subtle elements
box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);

// Medium shadow - cards, containers
box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);

// Large shadow - modals, important elements
box-shadow: 0 4px 16px rgba(0, 0, 0, 0.2);
```

## Typography

### Font Sizes

```scss
// Small text
font-size: 14px;

// Body text
font-size: 16px;

// Large body
font-size: 18px;

// Headings (responsive)
font-size: clamp(24px, 5vw, 30px);
font-size: clamp(24px, 5vw, 36px);
```

### Font Weights

```scss
font-weight: 400;  // Normal text (default)
font-weight: 500;  // Labels, emphasis
font-weight: 600;  // Buttons, strong emphasis
font-weight: 700;  // Headings, very strong emphasis
```

## Colors

```scss
// Primary
$dark-blue: #003865;
$darker-blue: #002d51;

// Neutral
$white: #ffffff;
$lighter-grey: #bdbdbd;
$light-grey: #929292;
$dark-grey: #1f1f1f;
$grey: #272727;
$black: #000000;

// Accent
$dark-red: #c10a27;
$darker-red: #a40a23;
```

## Responsive Breakpoints

```scss
// Mobile
@media (max-width: 450px) { }

// Tablet
@media (max-width: 768px) { }

// Desktop
@media (min-width: 768px) { }
@media (min-width: 1000px) { }
```

## Common Patterns

### Button

```scss
.my-button {
  @include Mixins.dark-button; // or light-button
  padding: 12px 24px;
  font-size: 18px;
  font-weight: 600;
  letter-spacing: 0.5px;
}
```

### Input Field

```scss
.my-input {
  border-radius: 8px;
  padding: 10px 14px;
  font-size: 18px;
  border: 1px solid $dark-blue;
  transition: all 0.3s ease;
  
  &:focus {
    outline: none;
    border-color: $dark-blue;
    box-shadow: 0 0 0 3px rgba(0, 56, 101, 0.1);
  }
}
```

### Card Container

```scss
.my-card {
  padding: 24px;
  background-color: $white;
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  max-width: 1200px;
  margin: 0 auto;
  
  @media (max-width: 768px) {
    padding: 20px;
  }
}
```

### Hover Effect

```scss
.my-element {
  transition: all 0.3s ease;
  
  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  }
}
```

### Focus State (Accessibility)

```scss
.my-interactive-element {
  &:focus-visible {
    outline: 3px solid $dark-blue;
    outline-offset: 2px;
  }
}
```

## Layout Patterns

### Centered Container

```scss
.container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
  
  @media (max-width: 768px) {
    padding: 16px;
  }
}
```

### Flexbox Column with Gap

```scss
.flex-column {
  display: flex;
  flex-direction: column;
  gap: 16px;
  align-items: center;
}
```

### Responsive Grid

```scss
.grid {
  display: flex;
  flex-direction: column;
  gap: 20px;
  
  @media (min-width: 768px) {
    flex-direction: row;
    flex-wrap: wrap;
  }
}
```

## Animation Guidelines

### Transition Speed

```scss
transition: all 0.3s ease; // Standard
transition: transform 0.3s ease; // Better performance
```

### Transform vs Position

```scss
// ✅ Good - GPU accelerated
transform: translateY(-2px);

// ❌ Avoid - causes layout recalculation
top: -2px;
```

## Accessibility Checklist

When creating new components:

- [ ] Focus states visible on all interactive elements
- [ ] Touch targets minimum 40px x 40px
- [ ] Color contrast ratio at least 4.5:1 for text
- [ ] Keyboard navigation works (Tab, Enter, Escape)
- [ ] Hover effects don't rely solely on color change
- [ ] Form labels associated with inputs
- [ ] Error messages clearly visible

## Common Mixins

```scss
@use 'utilities/Mixins';

// Clickable cursor
@include Mixins.clickable;

// Link style
@include Mixins.link;

// Dark button
@include Mixins.dark-button;

// Light button
@include Mixins.light-button;

// Table style
@include Mixins.table;
```

## CSS Custom Properties (Future Use)

Available variables for theming:

```css
var(--color-primary)
var(--color-primary-dark)
var(--color-white)
var(--color-grey-light)
var(--spacing-unit)
var(--border-radius-sm)
var(--border-radius-md)
var(--border-radius-lg)
var(--transition-speed)
var(--shadow-sm)
var(--shadow-md)
var(--shadow-lg)
```

## Testing Your Changes

Before committing UI changes:

1. **Build**: `npm run build`
2. **Format**: `npx prettier --write "src/**/*.scss"`
3. **Test Responsive**: Check at 375px, 768px, 1920px
4. **Test Keyboard**: Tab through all interactive elements
5. **Check Focus**: Verify all focus states are visible
6. **Test Hover**: Hover over all interactive elements
7. **Verify Colors**: Check contrast ratios

## Don't Do This ❌

```scss
// Magic numbers without explanation
padding: 13px 17px;

// Inline styles
<div style="margin: 10px;">

// Important without reason
color: red !important;

// Hardcoded colors
background-color: #bdbdbd; // Use $lighter-grey instead

// Non-standard spacing
margin: 15px; // Use 16px from spacing scale

// Missing transitions
.button:hover {
  background: blue; // Add transition!
}
```

## Do This Instead ✅

```scss
// Use spacing scale
padding: 12px 16px; // or 14px 20px

// Use SCSS variables
background-color: $lighter-grey;

// Add transitions
.button {
  transition: all 0.3s ease;
  
  &:hover {
    background: $darker-blue;
  }
}

// Use mixins for common patterns
.my-button {
  @include Mixins.dark-button;
  padding: 12px 24px;
}
```

## Quick Wins for Better UI

1. **Add transitions**: Makes everything feel smoother
2. **Use consistent spacing**: From the spacing scale
3. **Add hover effects**: Users love feedback
4. **Round corners**: 8px, 12px, or 16px
5. **Soften shadows**: Use rgba with low alpha
6. **Improve padding**: Give elements room to breathe
7. **Use font weights**: Create hierarchy (500, 600, 700)
8. **Make it responsive**: Test on mobile first

## Resources

- **Spacing**: Always use multiples of 4px
- **Colors**: Defined in `utilities/_Colors.scss`
- **Mixins**: Defined in `utilities/_Mixins.scss`
- **Documentation**: See `UI_UX_IMPROVEMENTS.md` for detailed explanations
- **Visual Guide**: See `VISUAL_CHANGES.md` for before/after comparisons

---

**Remember**: Consistency is key. Use this guide to maintain the polished, modern feel of the application.
