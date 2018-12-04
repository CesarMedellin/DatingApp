import { Photo } from './photo';

export interface User { // cada vez que se reciba
    id: number;
    username: string;
    knownAs: string;
    age: number;
    gender: string;
    created: Date;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[]; // aqui se dirige a la interfase de la foto porque el usuario recibe tambien datos de otra tabla
}
