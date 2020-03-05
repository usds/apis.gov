(async function() {
  
  const apiData = await getAPIData();
  buildAPIList(apiData);
  
  
  function buildAPIList(data) {
    const list = document.querySelector('.api-list');
    if (!list) { console.warn('There is no list to add APIs to!'); return; }
    
    let listItems = '';
    data.apis.forEach((api) => {
      listItems += `<li class="api-entry"><h3>Name: ${api.name}</h3><p>${api.description}</p></li>`
    });
    list.innerHTML += listItems;
  }
  
  async function getAPIData() {
    const response = await fetch('apis.json');
    const data = await response.json();
    return data;
  }
  
})();