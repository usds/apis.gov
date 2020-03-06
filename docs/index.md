---
layout: home
title: APIs.gov Registry
---


<h2 class="text-center">Find publicly available APIs offered by Federal Government Agencies</h2>


<!-- <form class="usa-search usa-search--small">
  <div role="search">
    <label class="usa-sr-only" for="search-field-small">Search</label>
    <input class="usa-input" id="search-field-small" type="search" name="search">
    <button class="usa-button" type="submit">
      <span class="usa-sr-only">Search</span>
    </button>
  </div>
</form> -->
<div>
  <ul class="usa-list usa-list--unstyled api-list" style="min-width: 90%;">
  </ul>
</div>

<script type="text/javascript">
(async function() {
  window.apigov.buildAPIList();
  // window.apigov.watchSearch();
})();
</script>
