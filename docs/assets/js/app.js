(async function() {
  window.apigov = (window.apigov || {});
  
  window.apigov.buildAPIList = async function buildAPIList() {
    const data = await getAllAPIs();
    const list = document.querySelector('.api-list');
    if (!list) { console.warn('There is no list to add APIs to!'); return; }
    
    let listItems = '';
    
    Object.keys(data).forEach((hash) => {
      listItems += `<li class="api-entry"><h3>Name: ${data[hash].name}</h3><p>${data[hash].description}</p></li>`
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