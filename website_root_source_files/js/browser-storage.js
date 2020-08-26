(function () {
    window.localstorageinterface = {
        setData: (name, data) => 
        {
            window.localStorage.setItem(name, data);
        },
        getData: (name) => 
        {
            return (window.localStorage.getItem(name));
        }
    }
})();