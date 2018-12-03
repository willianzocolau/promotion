import { Component, Output, EventEmitter } from '@angular/core';
import { Camera, CameraOptions } from '@ionic-native/camera';
import { LoadingController, AlertController } from "ionic-angular";
import { HTTP } from '@ionic-native/http';
import { ServerStrings } from "../../providers/serverStrings";
import { UserData } from "../../providers/userData";

@Component({
  selector: 'imgupload',
  templateUrl: 'imgupload.html'
})
export class ImguploadComponent {

  public link = "";
  public upload = false;
  @Output() imageUrl = new EventEmitter<string>();

  constructor(public camera: Camera,
    public loadingCtrl: LoadingController,
    public alertCtrl: AlertController,
    public http: HTTP,
    public server: ServerStrings,
    public user: UserData) {}

  getImage() {
    const options: CameraOptions = {
      quality: 70,
      destinationType: this.camera.DestinationType.DATA_URL,
      sourceType: this.camera.PictureSourceType.PHOTOLIBRARY,
      saveToPhotoAlbum:false
    }

    this.camera.getPicture(options).then((imageData) => {
      // imageData is either a base64 encoded string or a file URI
      // If it's base64:
      this.uploadImage(imageData);
    }, (err) => {
      // Handle error
    });
  }

  uploadImage(image :string){
    let loading = this.loadingCtrl.create({ content: 'Carregando imagem...' });
    loading.present();

    let endpoint: string = "https://api.imgur.com/3/image";
    let headers = {
      'Authorization': 'Client-ID a3e24d042148093',
    };

    this.http.setDataSerializer('utf8');
    this.http.post(endpoint, image, headers)
      .then(response => {
        let dados = JSON.parse(response.data);
        this.link = dados.data.link;
        this.imageUrl.emit(this.link);
        loading.dismiss();
        this.http.setDataSerializer('json');
      })
      .catch(exception => {
        console.log(exception);
        this.alertCtrl.create({ title: 'Erro: ' + exception.error, buttons: ['Ok'] }).present();
        loading.dismiss();
        this.http.setDataSerializer('json');
      });
  }
}
