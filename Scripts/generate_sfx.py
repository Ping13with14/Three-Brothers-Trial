"""游戏常用音效一键生成脚本。
基于 sfxr 思路，纯 Python 标准库实现，运行即生成 WAV 文件。
使用方法: python generate_sfx.py
输出目录: ../Assets/Audio/SFX/
"""

import math
import struct
import wave
import random
import os

SAMPLE_RATE = 44100
OUTPUT_DIR = os.path.join(os.path.dirname(__file__), "..", "Assets", "Audio", "SFX")


def write_wav(filepath, samples):
    """将采样列表写入 16-bit 单声道 WAV 文件"""
    os.makedirs(os.path.dirname(filepath), exist_ok=True)
    max_val = max(abs(s) for s in samples) or 1.0
    with wave.open(filepath, "w") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(SAMPLE_RATE)
        for s in samples:
            normalized = int(s / max_val * 32000)
            w.writeframes(struct.pack("<h", max(-32767, min(32767, normalized))))
    print(f"  已生成: {os.path.basename(filepath)}")


def envelope(t, duration, attack=0.0, decay=0.3, sustain=0.5, release=0.2):
    """ADSR 包络"""
    if t < attack * duration:
        return t / (attack * duration)
    elif t < (attack + decay) * duration:
        progress = (t - attack * duration) / (decay * duration)
        return 1.0 - (1.0 - sustain) * progress
    elif t < (attack + decay + release) * duration:
        return sustain
    else:
        return 0.0


def square_wave(freq, t):
    """方波（经典 8-bit 风格）"""
    return 1.0 if math.sin(2 * math.pi * freq * t) >= 0 else -1.0


def saw_wave(freq, t):
    """锯齿波"""
    return 2.0 * ((freq * t) % 1.0) - 1.0


def sine_wave(freq, t):
    """正弦波"""
    return math.sin(2 * math.pi * freq * t)


def noise():
    """白噪声"""
    return random.uniform(-1.0, 1.0)


def generate_pickup_coin():
    """拾取金币音效: 两段上升方波"""
    samples = []
    duration = 0.15
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        freq = 800 + 1200 * (t / duration)  # 频率从低到高
        env = 1.0 - t / duration
        samples.append(square_wave(freq, t) * env * 0.6)
    return samples


def generate_laser_shoot():
    """攻击/射击音效: 快速下降的锯齿波"""
    samples = []
    duration = 0.25
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        freq = 1200 - 1000 * (t / duration)  # 频率从高到低
        env = 1.0 - t / duration
        samples.append(saw_wave(freq, t) * env * 0.7)
    return samples


def generate_explosion():
    """爆炸/技能爆发音效: 噪声 + 低频方波混合"""
    samples = []
    duration = 0.5
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        progress = t / duration
        env = 1.0 - progress
        # 噪声分量
        n = noise() * env * 0.5
        # 低频方波 (震动感)
        b = square_wave(60 - 40 * progress, t) * env * 0.5
        samples.append(n + b)
    return samples


def generate_hit_hurt():
    """受击音效: 噪声 burst + 低频下降"""
    samples = []
    duration = 0.2
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        progress = t / duration
        freq = 300 - 250 * progress
        env = 1.0 - progress * progress
        n = noise() * env * 0.3
        s = square_wave(freq, t) * env * 0.5
        samples.append(n + s)
    return samples


def generate_powerup():
    """升级/强化音效: 三段上升方波"""
    samples = []
    duration = 0.4
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        progress = t / duration
        stage = int(progress * 3)
        base_freq = [400, 700, 1100][min(stage, 2)]
        freq = base_freq + 200 * progress
        env = 1.0 - progress * 0.5
        samples.append(square_wave(freq, t) * env * 0.5)
    return samples


def generate_blip_select():
    """UI 点击/选择音效: 两个短促方波"""
    samples = []
    duration = 0.06
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        freq = 600 + 400 * (t / duration)
        env = 1.0 - t / duration
        samples.append(square_wave(freq, t) * env * 0.5)
    return samples


def generate_jump():
    """跳跃音效: 快速上升的正弦波"""
    samples = []
    duration = 0.15
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        freq = 300 + 600 * (t / duration)
        env = 1.0 - t / duration
        samples.append(sine_wave(freq, t) * env * 0.6)
    return samples


def generate_bow_shoot():
    """弓箭射击音效: 快速嗖声"""
    samples = []
    duration = 0.3
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        progress = t / duration
        freq = 2000 - 1800 * progress
        env = 1.0 - progress * progress
        n = noise() * env * 0.2
        s = sine_wave(freq, t) * env * 0.6
        samples.append(n + s)
    return samples


def generate_death():
    """死亡音效: 长下降噪声"""
    samples = []
    duration = 0.8
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        progress = t / duration
        freq = 500 - 480 * progress
        env = 1.0 - progress
        n = noise() * env * 0.35
        s = saw_wave(freq, t) * env * 0.35
        samples.append(n + s)
    return samples


def generate_footstep():
    """脚步声: 短低频噪声"""
    samples = []
    duration = 0.08
    for i in range(int(SAMPLE_RATE * duration)):
        t = i / SAMPLE_RATE
        env = 1.0 - (t / duration) * (t / duration)
        samples.append(noise() * env * 0.4)
    return samples


SOUNDS = {
    "拾取金币_PickupCoin": generate_pickup_coin,
    "攻击_LaserShoot": generate_laser_shoot,
    "技能爆发_Explosion": generate_explosion,
    "受击_HitHurt": generate_hit_hurt,
    "升级_PowerUp": generate_powerup,
    "UI点击_BlipSelect": generate_blip_select,
    "跳跃_Jump": generate_jump,
    "弓箭射击_BowShoot": generate_bow_shoot,
    "死亡_Death": generate_death,
    "脚步_Footstep": generate_footstep,
}


def main():
    print(f"正在生成 {len(SOUNDS)} 个音效到: {OUTPUT_DIR}\n")
    for name, func in SOUNDS.items():
        write_wav(os.path.join(OUTPUT_DIR, f"{name}.wav"), func())
    print(f"\n✓ 全部完成! 共生成 {len(SOUNDS)} 个音效文件")


if __name__ == "__main__":
    main()
