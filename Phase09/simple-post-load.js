import http from 'k6/http';
import {check, sleep} from 'k6';

export const options = {
    vus: 20,
    duration: '1m',
};

const URL = 'http://localhost:5111/api/search';
const HEADERS = {'Content-Type': 'application/json'};
const BODY = JSON.stringify({
    "required": ["youth", "i have"]
});

export default function() {
    const res = http.post(URL, BODY, {headers: HEADERS});

    check(res, {
        'status is OK/Created/NoContent': (r) => [200, 201, 204].includes(r.status),
    });

    sleep(0.3);
}