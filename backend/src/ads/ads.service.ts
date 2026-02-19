import { Inject, Injectable } from '@nestjs/common';
import { CreateAdDto } from './dto/create-ad.dto';
import { UpdateAdDto } from './dto/update-ad.dto';
import { Ad } from './entities/ad.entity';
import { InjectModel } from '@nestjs/mongoose';
import { Model } from 'mongoose';
import { Client, ClientKafka, Transport } from '@nestjs/microservices';
import { Kafka } from 'kafkajs';


@Injectable()
export class AdsService {

  // @Client({
  //   transport: Transport.KAFKA,
  //   options: {
  //     client: {
  //       brokers: ['localhost:9093'],
  //     },
  //   },
  // })
  // private client: ClientKafka;

  private kafka = new Kafka({
    clientId: 'ad-publisher',
    // brokers: ['localhost:9093'],
    brokers: ['kafka:9093'],
  });

  private producer = this.kafka.producer();

  constructor(
    @InjectModel(Ad.name) private adModel: Model<Ad>,
  ) {
    this.connectProducer();
  }

  private async connectProducer() {
    try {
      await this.producer.connect();
      console.log('Producteur Kafka connecté');
    } catch (error) {
      console.error('Erreur de connexion au producteur Kafka:', error);
    }
  }

  async create(createAdDto: CreateAdDto) {
    const newAd = new this.adModel(createAdDto);
    const savedAd = await newAd.save();

    // Publier un événement sur Kafka après l'enregistrement
    // this.kafkaClient.emit('ad.created', { id: savedAd._id, ...createAdDto });

    // this.client.emit('ad.added', savedAd);

    try {
      await this.producer.send({
        topic: 'ad-notifications', // Ton topic Kafka
        messages: [
          {
            value: JSON.stringify({
              adId: savedAd._id,
              title: savedAd.title,
              createdAt: new Date(),
            }),
          },
        ],
      });
  
      console.log('Message Kafka envoyé pour l\'annonce', savedAd._id);
    } catch (error) {
      console.error('Erreur d\'envoi du message Kafka:', error);
    }

    return savedAd;

    // const newAd = new this.adModel(createAdDto);
    // return newAd.save();
  }

  findAll() {
    return this.adModel.find().exec();
  }

  findOne(id: string) {
    return this.adModel.findById(id).exec();
  }

  update(id: number, updateAdDto: UpdateAdDto) {
    return this.adModel.findByIdAndUpdate(id, updateAdDto, { new: true }).exec();
  }

  remove(id: number) {
    return this.adModel.findByIdAndDelete(id).exec();
  }
}
