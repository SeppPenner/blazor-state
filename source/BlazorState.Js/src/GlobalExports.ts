if (typeof window !== 'undefined' && !window['BlazorState']) {
  // When the library is loaded in a browser via a <script> element, make the
  // following APIs available in global scope for invocation from JS
  window['BlazorState'] = {
  };
}