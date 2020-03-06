(async function() {
  window.apigov = (window.apigov || {});

  window.apigov.watchSearch = async function watchSearch() {
    const search = document.querySelector('.search');

    search.addEventListener( 'keyup', function( e ) {
      var searchVal = search.value.trim().toLowerCase();
      var listItems = document.querySelectorAll('.api-entry');

      listItems.forEach(item => {
        var str = item.innerText.toLowerCase();

        if(str.match(searchVal)) {
          item.classList.remove('display-none');
        }
        else {
          item.classList.add('display-none');
        }
      });
    });
  }

  window.apigov.buildAPIList = async function buildAPIList() {
    const data = await getAllAPIs();
    const list = document.querySelector('.api-list');
    if (!list) { console.warn('There is no list to add APIs to!'); return; }

    let listItems = '';

    Object.keys(data).forEach((hash) => {
      var tags = data[hash].tags;
      var tagHTML = '';

      tags.forEach(tag => {
        tagHTML+= `<span class="usa-tag">${tag}</span>`
      });

      listItems += `<li class="api-entry margin-top-5">
      <div class="grid-row">
        <div class="grid-col-3"><a href="${data[hash].humanURL}" target="_blank">${data[hash].name}</a></div>
        <div class="grid-col-fill">${data[hash].description}</div>
        <div class="grid-col-3">${tagHTML}</div>
      </div>
      </li>`
    });
    list.innerHTML += listItems;
  }

  async function getAllAPIs() {
    let data = window.localStorage.getItem('apiData');
    const lastUpdate = window.localStorage.getItem('apiListUpdateTime');
    // make sure we have data, and it is less than a week old
    if (data && lastUpdate && Number(lastUpdate) > (Date.now() - (1000 * 60 * 60 * 24 * 7))) {
      try {
        data = JSON.parse(data);
      } catch(e) {
        console.warn('Unable to parse local API data:', e.message);
        return await getAPIData();
      }
      return data;
    }

    return await getAPIData();
  }

  async function getAPIData() {
    const response = await fetch('apis.json');
    const data = await response.json();
    window.localStorage.setItem('apiListUpdateTime', Date.now());
    return addDataLocally(data);
  }

  function addDataLocally(data) {
    const apiList = {};
    data.apis.forEach((api) => {
      apiList[hash(api.baseURL)] = api;
    });
    window.localStorage.setItem('apiData', JSON.stringify(apiList));
    return apiList;
  }

  function hash(apiUrl) {
    return apiUrl.replace(/[^a-z0-9]/g, '');
  }

})();