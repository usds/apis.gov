---
layout: home
title: APIs.gov Registry
---


<h2 class="text-center">Find publicly available APIs offered by Federal Government Agencies</h2>

<section>
  <div class="grid-row">
    <input class="margin-x-auto usa-input text-center search" id="search-field-small" type="search" name="search" placeholder="Search">
  </div>
</section>

<div>
  <ul class="usa-list usa-list--unstyled api-list" style="min-width: 90%;">
  </ul>
</div>

<script type="text/javascript">
(async function() {
  window.apigov.buildAPIList();
  window.apigov.watchSearch();
})();
</script>
