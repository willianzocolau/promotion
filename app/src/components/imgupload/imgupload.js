var request_url = 'https://api.imgur.com/3/image';
var client_id = 'a3e24d042148093';

function upload(file){
    var xhttp = new XMLHttpRequest();
    var fd = new FormData();
    var url_img = '';

    fd.append('image',file);
    
    xhttp.open('POST',request_url,true);
    xhttp.setRequestHeader('Authorization', 'Client-ID ' + client_id);
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4) {
            if (this.status >= 200 && this.status < 300) {
                var response = '';
                try{
                    response = JSON.parse(this.responseText);
                   url_img = response.data.link;
                   alert(url_img);
                }catch (err) {
                    response = this.responseText;
                }
            } else {
                throw new Error(this.status + " - " + this.statusText);
            }

        }
    };
    xhttp.send(fd);
    return url_img;
}