window.setData = (name, data) => 
{
    window.localStorage.setItem(name, data);
}

window.getData = (name) => 
{
    return window.localStorage.getItem(name);
}